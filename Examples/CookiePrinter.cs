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
            Chromium.Cookie[] chromiumCookies = results.Item1.ToArray(); // Simplify grabbed chromium cookies
            Firefox.Cookie[] firefoxCookies = results.Item2.ToArray(); // Simplify grabbed firefox cookies

            // Show chromium cookies:
            Console.WriteLine("CHROMIUM COOKIES:");
            foreach (var cg in chromiumCookies)
            {
                // Print the hostname, name, and value of the cookie:
                Console.WriteLine();
                Console.WriteLine($"Hostname: {cg.HostKey}");
                Console.WriteLine($"Name: {cg.Name}");
                Console.WriteLine($"EncValue: {cg.EncryptedValue}");
            }

            Console.WriteLine();

            // Show firefox cookies:
            Console.WriteLine("FIREFOX COOKIES:");
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
