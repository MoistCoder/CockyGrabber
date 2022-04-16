using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CockyGrabber.Utility.Cryptography.BouncyCastle
{
    public abstract class Arrays
    {
        /// <summary>
        /// Are two arrays equal.
        /// </summary>
        /// <param name="a">Left side.</param>
        /// <param name="b">Right side.</param>
        /// <returns>True if equal.</returns>
        public static bool AreEqual(byte[] a, byte[] b)
        {
            if (a == b)
                return true;

            if (a == null || b == null)
                return false;

            return HaveSameContents(a, b);
        }

        /// <summary>
        /// A constant time equals comparison - does not terminate early if
        /// test will fail.
        /// </summary>
        /// <param name="a">first array</param>
        /// <param name="b">second array</param>
        /// <returns>true if arrays equal, false otherwise.</returns>
        public static bool ConstantTimeAreEqual(byte[] a, byte[] b)
        {
            if (null == a || null == b)
                return false;
            if (a == b)
                return true;

            int len = System.Math.Min(a.Length, b.Length);
            int nonEqual = a.Length ^ b.Length;
            for (int i = 0; i < len; ++i)
            {
                nonEqual |= (a[i] ^ b[i]);
            }
            for (int i = len; i < b.Length; ++i)
            {
                nonEqual |= (b[i] ^ ~b[i]);
            }
            return 0 == nonEqual;
        }

        private static bool HaveSameContents(
            byte[] a,
            byte[] b)
        {
            int i = a.Length;
            if (i != b.Length)
                return false;
            while (i != 0)
            {
                --i;
                if (a[i] != b[i])
                    return false;
            }
            return true;
        }

        public static byte[] Clone(byte[] data)
        {
            return data == null ? null : (byte[])data.Clone();
        }

        [CLSCompliantAttribute(false)]
        public static ulong[] Clone(ulong[] data)
        {
            return data == null ? null : (ulong[])data.Clone();
        }

        public static void Fill(
            byte[] buf,
            byte b)
        {
            int i = buf.Length;
            while (i > 0)
            {
                buf[--i] = b;
            }
        }

        /**
         * Make a copy of a range of bytes from the passed in data array. The range can
         * extend beyond the end of the input array, in which case the return array will
         * be padded with zeroes.
         *
         * @param data the array from which the data is to be copied.
         * @param from the start index at which the copying should take place.
         * @param to the final index of the range (exclusive).
         *
         * @return a new byte array containing the range given.
         */
        public static byte[] CopyOfRange(byte[] data, int from, int to)
        {
            int newLength = GetLength(from, to);
            byte[] tmp = new byte[newLength];
            Array.Copy(data, from, tmp, 0, System.Math.Min(newLength, data.Length - from));
            return tmp;
        }

        private static int GetLength(int from, int to)
        {
            int newLength = to - from;
            if (newLength < 0)
                throw new ArgumentException(from + " > " + to);
            return newLength;
        }
    }
}
