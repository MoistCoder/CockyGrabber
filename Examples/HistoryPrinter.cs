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

            // Show Blink/Chromium site visits:
            Console.WriteLine("BLINK/CHROMIUM VISITS:");
            foreach (var v in g.GetAllBlinkHistories())
            {
                // Print the title and URL of every visited site:
                Console.WriteLine();
                Console.WriteLine($"Title: {v.Title}");
                Console.WriteLine($"Url: {v.Url}");
            }

            Console.WriteLine();

            // Show Gecko site visits:
            Console.WriteLine("GECKO VISITS:");
            foreach (var v in g.GetAllGeckoHistories())
            {
                // Print the title and URL of every visited site:
                Console.WriteLine();
                Console.WriteLine($"Title: {v.Title}");
                Console.WriteLine($"Url: {v.Url}");
            }

            System.Threading.Thread.Sleep(-1); // Pause console
        }
    }
}