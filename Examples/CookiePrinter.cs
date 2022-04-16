using CockyGrabber.Grabbers;
using System;

namespace CockyGrabberTest
{
    // This Program collects all Cookies and outputs them in a console window
    class Program
    {
        static void Main(string[] args)
        {
            UniversalGrabber g = new UniversalGrabber(); // Create Grabber

            // Show Blink/Chromium cookies:
            Console.WriteLine("BLINK/CHROMIUM COOKIES:");
            foreach (var c in g.GetAllBlinkCookies())
            {
                // Print the hostname, name, and value of the cookie:
                Console.WriteLine();
                Console.WriteLine($"Hostname: {c.HostKey}");
                Console.WriteLine($"Name: {c.Name}");
                Console.WriteLine($"Value: {c.DecryptedValue}");
            }

            Console.WriteLine();

            // Show Gecko cookies:
            Console.WriteLine("GECKO COOKIES:");
            foreach (var c in g.GetAllGeckoCookies())
            {
                // Print the hostname, name, and value of the cookie:
                Console.WriteLine();
                Console.WriteLine($"Hostname: {c.Host}");
                Console.WriteLine($"Name: {c.Name}");
                Console.WriteLine($"Value: {c.Value}");
            }

            System.Threading.Thread.Sleep(-1); // Pause console
        }
    }
}