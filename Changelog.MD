# 2.4.0

## Additions

<!--* Added a custom `SQLiteConnection` class which makes the use of System.Data.SQLite obsolete-->
* Added a Nuget Package
* Added a `ToJson()` method to all browser information classes (`Cookie`, `Login`, `Bookmark`, ...) that serializes the class into a JSON string
* Added Unit Tests for the different browser grabber classes.
* It's now possible to retrieve the form history and saved credit cards from browsers (for now, credit cards only from Blink-based browsers).
* CockyGrabber does now support the gathering of data on multiple blink browser profiles.
* The `BlinkGrabber` and `GeckoGrabber` classes now have two methods for each browser information that check if the related file that stores that information exists: `[name of the data]Exist(string)` and `Any[name of the data]Exist()` (e. g: `CookiesExist(string)` and `AnyCookiesExist()`). The former returns a bool indicating if the, to the information corresponding, file of a specified browser profile exists, and the latter returns a bool indicating if that file exists on any browser profile (given that the browser supports having different profiles).
* Added a `SameSiteType` enum in each of the Blink and Gecko cookie model classes, replacing the return type of the `SameSite` fields.

## Improvements

* Browser file paths are now dynamic (they now use `Environment.GetFolderPath(Appdata)` instead of `C:\\Users\\{Environment.UserName}\\AppData...`) and support other drive letters than `C:\`
* Improved the `UniversalGrabber` Get-methods by using `Parallel.ForEach` insead of `foreach` making them faster by a **LOT**.
* Changed the `GetKey()` method of the `BlinkGrabber` to virtual so that it can be overwritten by the user.
* Fixed a bug in the `GetKey()` method of `BlinkGrabber` class that caused it to crash when the JSON object `os_crypt`, that stores the value `encrypted_key`, contained other values than `encrypted_key`. *(Changed regex expression from `\"os_crypt\"\\s*:\\s*{\\s*\"encrypted_key\"\\s*:\\s*\".*?\"\\s*}` to `\"os_crypt\"\s*:\s*\{\s*.*?(?=\"encrypted_key)\"encrypted_key\"\s*:\s*\"(?<encKey>.*?)\"\s*\}`)*
* The Universal `GetAllBlink...()` methods are now checking if the related file and the `Local State` file, which stores the key, exist before grabing the information to avoid any errors
* Fixed the wrong expiry dates in the `GetGeckoCookies()` & `GetGeckoCookiesBy()` methods (Their cookie expiry dates used to always be: `01.01.1970 00:00:00 +00:00`)
* Some variables have been renamed to make them more readable
* The arrays which contain the browser classes in the `UniversalGrabber` (`BlinkGrabbers[]` and `GeckoGrabbers[]`) are now public instead of private so that the user can change them if needed (e.g. to add a new browser)
* The Timestamp conversion methods like `UnixTimeInMillisecondsToDate(milliSeconds)` or `WebkitTimeStampToDateTime(microSeconds)` in the EngineGrabbers have been moved to the new `Time` class in the `Utility` namespace
* Some paths in the grabber classes (e.g: `BraveGrabber`; `ChromeGrabber`; `OperaGrabber`) have changed

# 2.3.0

## Additions

* Added a custom implementation of the BouncyCastle library for value decryption making the use of the external library obsolete
* System.Security.Cryptography.ProtectedData is not being used anymore because of the new way of extracting the blink master key from `Local State`
* Added History, Downloads and Bookmark grabbing
* Item-classes like `Cookie`, `Login`, and `Site` can now be converted into strings with the `ToString()` method. On conversion, the `ToString()` method will return only the most important information

## Improvements

* Improved the GetKey() Method of the `BlinkGrabber` class
* Improved the Get Methods
* Improved the `UniversalGrabber`

# 2.2.0

## Additions

* `Chromium` changed to `Blink`, and `Firefox` changed to `Gecko`
* Added `BlinkGrabber` and `GeckoGrabber` which replace `ChromeGrabber` and `FirefoxGrabber`
* Added Firefox Login Decryption
* Added `GrabberException` which replaces `CookieDatabaseNotFoundException`, `LoginDatabaseNotFoundException`, and `LocalStateNotFoundException`

## Improvements

* Fixed the bug where you couldn't call the Method `GetAllChromiumCookies()` with the UniversalGrabber without it throwing an exception when at least one of the Browsers was not installed (same thing goes for `GetAllChromiumLogins()` and their Get-By equvalents)
* Moved the documentation from Readme.md to the Github Wiki
* Added support for mutliple Profiles on gecko based browsers like Firefox
* Changed Timestamps from `long` to `DateTimeOffset`
* Improved the use of the `DynamicJsonConverter`

# 2.1.0

## Additions

* Added Changelog _(Changelog.MD)_
* A custom JavaScriptConverter for JSON deserialization has been added, making the use of the Newtonsoft.JSON library obsolete
* Added a MIT license
* Added the `UniversalGrabber()` class which can grab multiple items at once
* Added custom exceptions: `CookieDatabaseNotFoundException()`, `LocalStateNotFoundException()`, `LoginDatabaseNotFoundException()`

## Improvements

* Get() functions now trigger the new custom exceptions when an error occurs
