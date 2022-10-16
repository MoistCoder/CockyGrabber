using CockyGrabber.Grabbers;

namespace CockyGrabber.Test.Models.Empty
{
    public class EmptyGeckoGrabber : GeckoGrabber
    {
        public override string ProfileDirPath
        {
            get
            {
                return string.Empty;
            }
        }
    }
}
