using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CockyGrabber.Utility.Cryptography.BouncyCastle
{
    internal class Check
    {
        internal static void DataLength(byte[] buf, int off, int len, string msg)
        {
            if (off > (buf.Length - len))
                throw new Exception(msg);
        }

        internal static void OutputLength(byte[] buf, int off, int len, string msg)
        {
            if (off > (buf.Length - len))
                throw new Exception(msg);
        }
    }
}
