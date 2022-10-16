using System.Collections.Generic;
using System.Linq;
using CockyGrabber.Grabbers;
using static CockyGrabber.Test.Globals;

namespace CockyGrabber.Test
{
    public static class Tools
    {
        public static BlinkGrabber GetAnyBlinkBrowser()
        {
            foreach (BlinkGrabber g in UG.BlinkGrabbers)
            {
                if (g.EverythingExists()) return g;
            }
            return null;
        }
        public static GeckoGrabber GetAnyGeckoBrowser()
        {
            foreach (GeckoGrabber g in UG.GeckoGrabbers)
            {
                if (g.EverythingExists()) return g;
            }
            return null;
        }
        public static byte[] GetKeyFromAnyBlinkBrowser()
        {
            return GetAnyBlinkBrowser().GetKey();
        }

        public static T GetRandom<T>(this IEnumerable<T> x)
        {
            return x.ToArray()[Rnd.Next(0, x.Count())];
        }
    }
}
