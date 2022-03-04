# 2.1.0

## Additions

* Added Changelog _(Changelog.MD)_
* A custom JavaScriptConverter for JSON deserialization has been added, making the use of the Newtonsoft.JSON library obsolete
* Added a MIT license
* Added the `UniversalGrabber()` class which can grab multiple items at once
* Added custom exceptions: `CookieDatabaseNotFoundException()`, `LocalStateNotFoundException()`, `LoginDatabaseNotFoundException()`

## Improvements

* Get() functions now trigger the new custom exceptions when an error occurs
