using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CockyGrabber.Test.Models.Empty;
using static CockyGrabber.Test.Tools;

namespace CockyGrabber.Test.Tests.GrabberTests
{
    [TestClass]
    public class EmptyGrabberTests
    {
        [TestMethod]
        public void TestBlinkLocalStateNotFound()
        {
            try
            {
                EmptyBlinkGrabber g = new EmptyBlinkGrabber();
                g.GetKey();
                Assert.Fail();
            }
            catch (GrabberException e)
            {
                Assert.AreEqual(e.Error, GrabberError.LocalStateNotFound);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        [TestMethod]
        public void TestBlinkCookiesNotFound()
        {
            try
            {
                EmptyBlinkGrabber g = new EmptyBlinkGrabber();
                g.GetCookies(GetKeyFromAnyBlinkBrowser());
                Assert.Fail();
            }
            catch (GrabberException e)
            {
                Assert.AreEqual(e.Error, GrabberError.CookiesNotFound);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        [TestMethod]
        public void TestBlinkLoginsNotFound()
        {
            try
            {
                EmptyBlinkGrabber g = new EmptyBlinkGrabber();
                g.GetLogins(GetKeyFromAnyBlinkBrowser());
                Assert.Fail();
            }
            catch (GrabberException e)
            {
                Assert.AreEqual(e.Error, GrabberError.LoginsNotFound);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        [TestMethod]
        public void TestGeckoProfilesNotFound()
        {
            try
            {
                EmptyGeckoGrabber g = new EmptyGeckoGrabber();
                Assert.Fail();
            }
            catch (GrabberException e)
            {
                Assert.AreEqual(e.Error, GrabberError.ProfilesNotFound);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}