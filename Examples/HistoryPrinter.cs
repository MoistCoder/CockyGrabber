using CockyGrabber.Grabbers;
using System;

namespace CockyGrabberTest
{
    // This Program collects all broswer histories and outputs them in a console window
    class Program
    {
        static void Main(string[] args)
        {
            UniversalGrabber g = new UniversalGrabber(); // Create Grabber

            // Show Blink/Chromium histories:
            Console.WriteLine("BLINK/CHROMIUM VISITS:");
            foreach (var c in g.GetAllBlinkHistories())
            {
                // Print the title and URL of every visited site:
                Console.WriteLine();
                Console.WriteLine($"Title: {c.Title}");
                Console.WriteLine($"Url: {c.Url}");
            }

            Console.WriteLine();

            // Show Gecko histories:
            Console.WriteLine("GECKO VISITS:");
            foreach (var c in g.GetAllGeckoHistories())
            {
                // Print the title and URL of every visited site:
                Console.WriteLine();
                Console.WriteLine($"Title: {c.Title}");
                Console.WriteLine($"Url: {c.Url}");
            }

            System.Threading.Thread.Sleep(-1); // Pause console
        }
    }
}