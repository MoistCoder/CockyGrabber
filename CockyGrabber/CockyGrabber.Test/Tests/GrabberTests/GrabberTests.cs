using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using CockyGrabber.Grabbers;
using System;

namespace CockyGrabber.Test.Tests.GrabberTests
{
    [TestClass]
    public class GrabberTests
    {
        private BlinkGrabber _blinkGrabber;
        private GeckoGrabber _geckoGrabber;

        [TestInitialize]
        public void Setup()
        {
            _blinkGrabber = Tools.GetAnyBlinkBrowser();
            _geckoGrabber = Tools.GetAnyGeckoBrowser();
        }

        #region GetBookmarks() Tests
        [TestMethod]
        public void TestBlinkGrabberGetBookmarks()
        {
            Console.WriteLine($"[Testing with {_blinkGrabber.GetType().Name}]");
            if (!_blinkGrabber.AnyBookmarksExist())
            {
                Assert.Fail();
                return;
            }
            var x = _blinkGrabber.GetBookmarks();
            Assert.IsTrue(x.Count() > 0, "Bookmarks couldn't be grabbed!");
            Console.WriteLine(x.GetRandom());
        }
        [TestMethod]
        public void TestGeckoGrabberGetBookmarks()
        {
            Console.WriteLine($"[Testing with {_geckoGrabber.GetType().Name}]");
            var x = _geckoGrabber.GetBookmarks();
            Assert.IsTrue(x.Count() > 0, "Bookmarks couldn't be grabbed!");
            Console.WriteLine(x.GetRandom());
        }
        #endregion

        #region GetDownloads() Tests
        [TestMethod]
        public void TestBlinkGrabberGetDownloads()
        {
            Console.WriteLine($"[Testing with {_blinkGrabber.GetType().Name}]");
            if (!_blinkGrabber.AnyDownloadsExist())
            {
                Assert.Fail();
                return;
            }
            var x = _blinkGrabber.GetDownloads();
            Assert.IsTrue(x.Count() > 0, "Downloads couldn't be grabbed!");
            Console.WriteLine(x.GetRandom());
        }
        [TestMethod]
        public void TestGeckoGrabberGetDownloads()
        {
            Console.WriteLine($"[Testing with {_geckoGrabber.GetType().Name}]");
            var x = _geckoGrabber.GetDownloads();
            Assert.IsTrue(x.Count() > 0, "Downloads couldn't be grabbed!");
            Console.WriteLine(x.GetRandom());
        }
        #endregion

        #region GetHistory() Tests
        [TestMethod]
        public void TestBlinkGrabberGetHistory()
        {
            Console.WriteLine($"[Testing with {_blinkGrabber.GetType().Name}]");
            if (!_blinkGrabber.AnyHistoryExists())
            {
                Assert.Fail();
                return;
            }
            var x = _blinkGrabber.GetHistory();
            Assert.IsTrue(x.Count() > 0, "History couldn't be grabbed!");
            Console.WriteLine(x.GetRandom());
        }
        [TestMethod]
        public void TestGeckoGrabberGetHistory()
        {
            Console.WriteLine($"[Testing with {_geckoGrabber.GetType().Name}]");
            var x = _geckoGrabber.GetHistory();
            Assert.IsTrue(x.Count() > 0, "History couldn't be grabbed!");
            Console.WriteLine(x.GetRandom());
        }
        #endregion
        
        #region GetLogins() Tests
        [TestMethod]
        public void TestBlinkGrabberGetLogins()
        {
            Console.WriteLine($"[Testing with {_blinkGrabber.GetType().Name}]");
            if (!_blinkGrabber.AnyLoginsExist())
            {
                Assert.Fail();
                return;
            }
            var x = _blinkGrabber.GetLogins();
            Assert.IsTrue(x.Count() > 0, "Logins couldn't be grabbed!");
            Console.WriteLine(x.GetRandom());
        }
        [TestMethod]
        public void TestGeckoGrabberGetLogins()
        {
            Console.WriteLine($"[Testing with {_geckoGrabber.GetType().Name}]");
            var x = _geckoGrabber.GetLogins();
            Assert.IsTrue(x.Count() > 0, "Logins couldn't be grabbed!");
            Console.WriteLine(x.GetRandom());
        }
        #endregion

        #region GetCookies() Tests
        [TestMethod]
        public void TestBlinkGrabberGetCookies()
        {
            Console.WriteLine($"[Testing with {_blinkGrabber.GetType().Name}]");
            if (!_blinkGrabber.AnyCookiesExist())
            {
                Assert.Fail();
                return;
            }
            var x = _blinkGrabber.GetCookies();
            Assert.IsTrue(x.Count() > 0, "Cookies couldn't be grabbed!");
            Console.WriteLine(x.GetRandom());
        }
        [TestMethod]
        public void TestGeckoGrabberGetCookies()
        {
            Console.WriteLine($"[Testing with {_geckoGrabber.GetType().Name}]");
            var x = _geckoGrabber.GetCookies();
            Assert.IsTrue(x.Count() > 0, "Cookies couldn't be grabbed!");
            Console.WriteLine(x.GetRandom());
        }
        #endregion

        #region GetFormHistory() Tests
        [TestMethod]
        public void TestBlinkGrabberGetFormHistory()
        {
            Console.WriteLine($"[Testing with {_blinkGrabber.GetType().Name}]");
            if (!_blinkGrabber.AnyWebDataExists())
            {
                Assert.Fail();
                return;
            }
            var x = _blinkGrabber.GetFormHistory();
            Assert.IsTrue(x.Count() > 0, "FormHistory couldn't be grabbed!");
            Console.WriteLine(x.GetRandom());
        }
        [TestMethod]
        public void TestGeckoGrabberGetFormHistory()
        {
            Console.WriteLine($"[Testing with {_geckoGrabber.GetType().Name}]");
            var x = _geckoGrabber.GetFormHistory();
            Assert.IsTrue(x.Count() > 0, "FormHistory couldn't be grabbed!");
            Console.WriteLine(x.GetRandom());
        }
        #endregion

        #region GetCreditCards() Tests
        [TestMethod]
        public void TestBlinkGrabberGetCreditCards()
        {
            Console.WriteLine($"[Testing with {_blinkGrabber.GetType().Name}]");
            if (!_blinkGrabber.AnyWebDataExists())
            {
                Assert.Fail();
                return;
            }
            var x = _blinkGrabber.GetCreditCards();
            Assert.IsTrue(x.Count() > 0, "Credit Cards couldn't be grabbed!");
            Console.WriteLine(x.GetRandom());
        }
        [TestMethod]
        public void TestGeckoGrabberGetCreditCards()
        {
            Console.WriteLine($"[Testing with {_geckoGrabber.GetType().Name}]");
            if (!_geckoGrabber.AnyAutoFillProfilesExists())
            {
                Assert.Fail();
                return;
            }
            var x = _geckoGrabber.GetCreditCards();
            Assert.IsTrue(x.Count() > 0, "Credit Cards couldn't be grabbed!");
            Console.WriteLine(x.GetRandom());
        }
        #endregion
    }
}