using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CockyGrabber.Utility.Cryptography.BouncyCastle
{
    internal abstract class Bits
    {
        internal static ulong BitPermuteStep(ulong x, ulong m, int s)
        {
            ulong t = (x ^ (x >> s)) & m;
            return (t ^ (t << s)) ^ x;
        }

        internal static ulong BitPermuteStepSimple(ulong x, ulong m, int s)
        {
            return ((x & m) << s) | ((x >> s) & m);
        }
    }
}
