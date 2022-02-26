# CockyGrabber

CockyGrabber is a C#.NET library for collecting browser information such as cookies, logins, and more.
It's also very *easy* to integrate into your Projects.

## Table Of Contents

1. [Integration](#integration)
2. [Usage](#usage)
    * [Importing CockyGrabber](#importing-cockygrabber)
    * [Grabbing Cookies](#grabbing-cookies)
    * [Grabbing Logins](#grabbing-logins)
3. [What's Next](#whats-next)
4. [End](#end)

## Integration

Before you can use CockyGrabber you will need to download a few necessary packages. You can get them from external sources or easily with [NuGet](https://www.nuget.org/):

* [Newtonsoft.Json](https://www.newtonsoft.com/json)
* [BouncyCastle](http://www.bouncycastle.org/csharp/)
* [System.Data.SQLite](https://system.data.sqlite.org/)
* [System.Security.Cryptography.ProtectedData](http://www.dot.net/)

<!--Or you can download CockyGrabber directly from NuGet with the necessary packages included: [LINK]-->

## Usage

### Importing CockyGrabber

```cs
using CockyGrabber;
using CockyGrabber.Grabbers;
```

### Grabbing Cookies

Grabbing stuff with CockyGrabber is really easy!

To set an example here is how to collect Chrome cookies:

```cs
ChromeGrabber grabber = new ChromeGrabber(); // Define Grabber
List<Chromium.Cookie> cookies = grabber.GetCookies(); // Collect all Cookies with GetCookies()

// Print Hostname, Name and Value of every cookie:
foreach(Chromium.Cookie cookie in cookies) // Since Chrome is a Chromium-based Browser it uses Chromium Cookies
{
    string cookieHostname = cookie.HostKey;
    string cookieName = cookie.Name;
    string cookieValue = cookie.EncryptedValue;
    Console.WriteLine($"{cookieHostname} = {cookieName}: {cookieValue}");
}
```

*To collect the Cookies of any othe Chromium-based Browser just replace `ChromeGrabber` with `OperaGxGrabber`, `BraveGrabber`, and so on...*

</br>

Browsers like Firefox, which are based on other engines have their own classes, such as 'Firefox.Cookie' since they aren't Chromium-based:

```cs
FirefoxGrabber grabber = new FirefoxGrabber(); // Define Grabber
List<Firefox.Cookie> cookies = grabber.GetCookies(); // Collect all Cookies with GetCookies()

// Print Hostname, Name and Value of every cookie:
foreach(Firefox.Cookie cookie in cookies) // Firefox has its own engine and therefore its own Cookie class (Firefox.Cookie)
{
    string cookieHostname = cookie.Host;
    string cookieName = cookie.Name;
    string cookieValue = cookie.Value;
    Console.WriteLine($"{cookieHostname} = {cookieName}: {cookieValue}");
}
```

### Grabbing Logins

CockyGrabber can also grab Login Data such as Usernames and Passwords.

Here is an example with the `BraveGrabber()`:

```cs
BraveGrabber grabber = new BraveGrabber(); // Define Grabber
List<Chromium.Login> logins = grabber.GetLogins(); // Collect all Logins with GetLogins()

// Print the Origin(URL), Username value and Password value of every Login:
foreach(Chromium.Login login in logins) // Since Brave is a Chromium-based Browser it uses Chromium Logins
{
    string loginUrl = login.OriginUrl;
    string loginUsername = login.UsernameValue;
    string loginPassword = login.PasswordValue;
    Console.WriteLine($"{loginUrl} = {loginUsername}:{loginPassword}");
}
```

*Same thing goes here if you want to collect OperaGx, Chrome or Edge Cookies just replace `BraveGrabber` with `OperaGxGrabber`, `EdgeGrabber`, ...*

</br>

... And if you want to grab Logins from non-Chromium based browsers like Firefox then you'll need to use a special class like `Firefox.Login`:

```cs
FirefoxGrabber grabber = new FirefoxGrabber();
List<Firefox.Login> logins = grabber.GetLogins();

foreach(Firefox.Login login in logins)
// ...
```

#### **If you need more examples, then go to the Examples folder!**

## What's Next

1. Adding a Function that can grab all the data at once
2. Adding more things to grab like bookmarks, extensions, ...
3. Async Funtions
4. Turn CockyGrabber into a NuGet Package
5. Adding custom Functions that replace the packages
6. Creating a minimalized File that anyone can easily implement in their Project without referencing CockyGrabber itself
7. Migrate to NET Core
8. Create a better documentation
9. More Examples

## End

Thats it for now!

</br>

*CockyGrabber is still in development and will receive future updates* so if you found any **Bugs**, please create an Issue and report it!
