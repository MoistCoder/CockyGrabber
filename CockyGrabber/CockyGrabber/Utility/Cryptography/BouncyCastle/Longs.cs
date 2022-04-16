using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CockyGrabber.Utility.Cryptography.BouncyCastle
{
    public abstract class Longs
    {
        public static ulong Reverse(ulong i)
        {
            i = Bits.BitPermuteStepSimple(i, 0x5555555555555555UL, 1);
            i = Bits.BitPermuteStepSimple(i, 0x3333333333333333UL, 2);
            i = Bits.BitPermuteStepSimple(i, 0x0F0F0F0F0F0F0F0FUL, 4);
            return ReverseBytes(i);
        }

        public static ulong ReverseBytes(ulong i)
        {
            return RotateLeft(i & 0xFF000000FF000000UL, 8) |
                   RotateLeft(i & 0x00FF000000FF0000UL, 24) |
                   RotateLeft(i & 0x0000FF000000FF00UL, 40) |
                   RotateLeft(i & 0x000000FF000000FFUL, 56);
        }

        public static ulong RotateLeft(ulong i, int distance)
        {
            return (i << distance) ^ (i >> -distance);
        }
    }
}
