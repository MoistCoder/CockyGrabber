using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CockyGrabber.Utility.Cryptography.BouncyCastle
{
    internal abstract class GcmUtilities
    {
        private const uint E1 = 0xe1000000;
        private const ulong E1UL = (ulong)E1 << 32;

        internal static ulong[] OneAsUlongs()
        {
            ulong[] tmp = new ulong[2];
            tmp[0] = 1UL << 63;
            return tmp;
        }

        internal static void AsBytes(ulong[] x, byte[] z)
        {
            Pack.UInt64_To_BE(x, z, 0);
        }

        internal static ulong[] AsUlongs(byte[] x)
        {
            ulong[] z = new ulong[2];
            Pack.BE_To_UInt64(x, 0, z);
            return z;
        }

        internal static void AsUlongs(byte[] x, ulong[] z, int zOff)
        {
            Pack.BE_To_UInt64(x, 0, z, zOff, 2);
        }

        internal static void DivideP(ulong[] x, int xOff, ulong[] z, int zOff)
        {
            ulong x0 = x[xOff + 0], x1 = x[xOff + 1];
            ulong m = (ulong)((long)x0 >> 63);
            x0 ^= (m & E1UL);
            z[zOff + 0] = (x0 << 1) | (x1 >> 63);
            z[zOff + 1] = (x1 << 1) | (ulong)(-(long)m);
        }

        internal static void Multiply(byte[] x, byte[] y)
        {
            ulong[] t1 = GcmUtilities.AsUlongs(x);
            ulong[] t2 = GcmUtilities.AsUlongs(y);
            GcmUtilities.Multiply(t1, t2);
            GcmUtilities.AsBytes(t1, x);
        }

        internal static void Multiply(ulong[] x, ulong[] y)
        {
            //ulong x0 = x[0], x1 = x[1];
            //ulong y0 = y[0], y1 = y[1];
            //ulong z0 = 0, z1 = 0, z2 = 0;

            //for (int j = 0; j < 64; ++j)
            //{
            //    ulong m0 = (ulong)((long)x0 >> 63); x0 <<= 1;
            //    z0 ^= (y0 & m0);
            //    z1 ^= (y1 & m0);

            //    ulong m1 = (ulong)((long)x1 >> 63); x1 <<= 1;
            //    z1 ^= (y0 & m1);
            //    z2 ^= (y1 & m1);

            //    ulong c = (ulong)((long)(y1 << 63) >> 8);
            //    y1 = (y1 >> 1) | (y0 << 63);
            //    y0 = (y0 >> 1) ^ (c & E1UL);
            //}

            //z0 ^= z2 ^ (z2 >>  1) ^ (z2 >>  2) ^ (z2 >>  7);
            //z1 ^=      (z2 << 63) ^ (z2 << 62) ^ (z2 << 57);

            //x[0] = z0;
            //x[1] = z1;

            /*
             * "Three-way recursion" as described in "Batch binary Edwards", Daniel J. Bernstein.
             *
             * Without access to the high part of a 64x64 product x * y, we use a bit reversal to calculate it:
             *     rev(x) * rev(y) == rev((x * y) << 1) 
             */

            ulong x0 = x[0], x1 = x[1];
            ulong y0 = y[0], y1 = y[1];
            ulong x0r = Longs.Reverse(x0), x1r = Longs.Reverse(x1);
            ulong y0r = Longs.Reverse(y0), y1r = Longs.Reverse(y1);

            ulong h0 = Longs.Reverse(ImplMul64(x0r, y0r));
            ulong h1 = ImplMul64(x0, y0) << 1;
            ulong h2 = Longs.Reverse(ImplMul64(x1r, y1r));
            ulong h3 = ImplMul64(x1, y1) << 1;
            ulong h4 = Longs.Reverse(ImplMul64(x0r ^ x1r, y0r ^ y1r));
            ulong h5 = ImplMul64(x0 ^ x1, y0 ^ y1) << 1;

            ulong z0 = h0;
            ulong z1 = h1 ^ h0 ^ h2 ^ h4;
            ulong z2 = h2 ^ h1 ^ h3 ^ h5;
            ulong z3 = h3;

            z1 ^= z3 ^ (z3 >> 1) ^ (z3 >> 2) ^ (z3 >> 7);
            //          z2 ^=      (z3 << 63) ^ (z3 << 62) ^ (z3 << 57);
            z2 ^= (z3 << 62) ^ (z3 << 57);

            z0 ^= z2 ^ (z2 >> 1) ^ (z2 >> 2) ^ (z2 >> 7);
            z1 ^= (z2 << 63) ^ (z2 << 62) ^ (z2 << 57);

            x[0] = z0;
            x[1] = z1;
        }

        internal static void MultiplyP7(ulong[] x, int xOff, ulong[] z, int zOff)
        {
            ulong x0 = x[xOff + 0], x1 = x[xOff + 1];
            ulong c = x1 << 57;
            z[zOff + 0] = (x0 >> 7) ^ c ^ (c >> 1) ^ (c >> 2) ^ (c >> 7);
            z[zOff + 1] = (x1 >> 7) | (x0 << 57);
        }

        internal static void Square(ulong[] x, ulong[] z)
        {
            ulong[] t = new ulong[4];
            Expand64To128Rev(x[0], t, 0);
            Expand64To128Rev(x[1], t, 2);

            ulong z0 = t[0], z1 = t[1], z2 = t[2], z3 = t[3];

            z1 ^= z3 ^ (z3 >> 1) ^ (z3 >> 2) ^ (z3 >> 7);
            //          z2 ^=      (z3 << 63) ^ (z3 << 62) ^ (z3 << 57);
            z2 ^= (z3 << 62) ^ (z3 << 57);

            z0 ^= z2 ^ (z2 >> 1) ^ (z2 >> 2) ^ (z2 >> 7);
            z1 ^= (z2 << 63) ^ (z2 << 62) ^ (z2 << 57);

            z[0] = z0;
            z[1] = z1;
        }

        private const ulong M64R = 0xAAAAAAAAAAAAAAAAUL;
        internal static void Expand64To128Rev(ulong x, ulong[] z, int zOff)
        {
            // "shuffle" low half to even bits and high half to odd bits
            x = Bits.BitPermuteStep(x, 0x00000000FFFF0000UL, 16);
            x = Bits.BitPermuteStep(x, 0x0000FF000000FF00UL, 8);
            x = Bits.BitPermuteStep(x, 0x00F000F000F000F0UL, 4);
            x = Bits.BitPermuteStep(x, 0x0C0C0C0C0C0C0C0CUL, 2);
            x = Bits.BitPermuteStep(x, 0x2222222222222222UL, 1);

            z[zOff] = (x) & M64R;
            z[zOff + 1] = (x << 1) & M64R;
        }

        internal static void Xor(byte[] x, byte[] y)
        {
            int i = 0;
            do
            {
                x[i] ^= y[i]; ++i;
                x[i] ^= y[i]; ++i;
                x[i] ^= y[i]; ++i;
                x[i] ^= y[i]; ++i;
            }
            while (i < 16);
        }

        internal static void Xor(byte[] x, byte[] y, int yOff)
        {
            int i = 0;
            do
            {
                x[i] ^= y[yOff + i]; ++i;
                x[i] ^= y[yOff + i]; ++i;
                x[i] ^= y[yOff + i]; ++i;
                x[i] ^= y[yOff + i]; ++i;
            }
            while (i < 16);
        }

        internal static void Xor(byte[] x, int xOff, byte[] y, int yOff, byte[] z, int zOff)
        {
            int i = 0;
            do
            {
                z[zOff + i] = (byte)(x[xOff + i] ^ y[yOff + i]); ++i;
                z[zOff + i] = (byte)(x[xOff + i] ^ y[yOff + i]); ++i;
                z[zOff + i] = (byte)(x[xOff + i] ^ y[yOff + i]); ++i;
                z[zOff + i] = (byte)(x[xOff + i] ^ y[yOff + i]); ++i;
            }
            while (i < 16);
        }

        internal static void Xor(byte[] x, byte[] y, int yOff, int yLen)
        {
            while (--yLen >= 0)
            {
                x[yLen] ^= y[yOff + yLen];
            }
        }

        internal static void Xor(byte[] x, int xOff, byte[] y, int yOff, int len)
        {
            while (--len >= 0)
            {
                x[xOff + len] ^= y[yOff + len];
            }
        }

        internal static void Xor(ulong[] x, int xOff, ulong[] y, int yOff, ulong[] z, int zOff)
        {
            z[zOff + 0] = x[xOff + 0] ^ y[yOff + 0];
            z[zOff + 1] = x[xOff + 1] ^ y[yOff + 1];
        }

        private static ulong ImplMul64(ulong x, ulong y)
        {
            ulong x0 = x & 0x1111111111111111UL;
            ulong x1 = x & 0x2222222222222222UL;
            ulong x2 = x & 0x4444444444444444UL;
            ulong x3 = x & 0x8888888888888888UL;

            ulong y0 = y & 0x1111111111111111UL;
            ulong y1 = y & 0x2222222222222222UL;
            ulong y2 = y & 0x4444444444444444UL;
            ulong y3 = y & 0x8888888888888888UL;

            ulong z0 = (x0 * y0) ^ (x1 * y3) ^ (x2 * y2) ^ (x3 * y1);
            ulong z1 = (x0 * y1) ^ (x1 * y0) ^ (x2 * y3) ^ (x3 * y2);
            ulong z2 = (x0 * y2) ^ (x1 * y1) ^ (x2 * y0) ^ (x3 * y3);
            ulong z3 = (x0 * y3) ^ (x1 * y2) ^ (x2 * y1) ^ (x3 * y0);

            z0 &= 0x1111111111111111UL;
            z1 &= 0x2222222222222222UL;
            z2 &= 0x4444444444444444UL;
            z3 &= 0x8888888888888888UL;

            return z0 | z1 | z2 | z3;
        }
    }
}
