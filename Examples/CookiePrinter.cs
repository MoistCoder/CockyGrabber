using CockyGrabber;
using CockyGrabber.Grabbers;
using System;
using System.Linq;

namespace CockyGrabberTest
{
    // This Program collects all Cookies and outputs them in a console window
    class Program
    {
        static void Main(string[] args)
        {
            UniversalGrabber g = new UniversalGrabber(); // Create Grabber

            var results = g.GetAllCookies(); // Get ALL Cookies
            Blink.Cookie[] chromiumCookies = results.Item1.ToArray(); // Simplify grabbed Blink/Chromium cookies
            Gecko.Cookie[] firefoxCookies = results.Item2.ToArray(); // Simplify grabbed Gecko cookies

            // Show Blink/Chromium cookies:
            Console.WriteLine("BLINK/CHROMIUM COOKIES:");
            foreach (var cg in chromiumCookies)
            {
                // Print the hostname, name, and value of the cookie:
                Console.WriteLine();
                Console.WriteLine($"Hostname: {cg.HostKey}");
                Console.WriteLine($"Name: {cg.Name}");
                Console.WriteLine($"Value: {cg.DecryptedValue}");
            }

            Console.WriteLine();

            // Show Gecko cookies:
            Console.WriteLine("GECKO COOKIES:");
            foreach (var cg in firefoxCookies)
            {
                // Print the hostname, name, and value of the cookie:
                Console.WriteLine();
                Console.WriteLine($"Hostname: {cg.Host}");
                Console.WriteLine($"Name: {cg.Name}");
                Console.WriteLine($"Value: {cg.Value}");
            }

            System.Threading.Thread.Sleep(-1); // Pause console
        }
    }
}
