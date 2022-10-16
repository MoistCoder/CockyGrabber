using CockyGrabber.Grabbers;

namespace CockyGrabber.Test.Models.Empty
{
    public class EmptyBlinkGrabber : BlinkGrabber
    {
        public override string DataRootPath
        {
            get
            {
                string dir = Tools.GetAnyBlinkBrowser().Profiles[0];
                dir = dir.Remove(dir.LastIndexOf('\\'));
                return dir;
            }
        }
        public override string LocalStatePath
        {
            get
            {
                return @"\Lorem Ipsum\Fake Path";
            }
        }
        public override string CookiePath
        {
            get
            {
                return @"\Lorem Ipsum\Fake Path";
            }
        }
        public override string LoginDataPath
        {
            get
            {
                return @"\Lorem Ipsum\Fake Path";
            }
        }
        public override string HistoryPath
        {
            get
            {
                return @"\Lorem Ipsum\Fake Path";
            }
        }
        public override string BookmarkPath
        {
            get
            {
                return @"\Lorem Ipsum\Fake Path";
            }
        }
        public override string WebDataPath
        {
            get
            {
                return @"\Lorem Ipsum\Fake Path";
            }
        }
    }
}
