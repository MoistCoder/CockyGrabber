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

</br>

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

</br>

# 2.1.0

## Additions

* Added Changelog _(Changelog.MD)_
* A custom JavaScriptConverter for JSON deserialization has been added, making the use of the Newtonsoft.JSON library obsolete
* Added a MIT license
* Added the `UniversalGrabber()` class which can grab multiple items at once
* Added custom exceptions: `CookieDatabaseNotFoundException()`, `LocalStateNotFoundException()`, `LoginDatabaseNotFoundException()`

## Improvements

* Get() functions now trigger the new custom exceptions when an error occurs
