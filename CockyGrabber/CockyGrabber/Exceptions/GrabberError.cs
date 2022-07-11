namespace CockyGrabber
{
    public enum GrabberError
    {
        UnknownError,

        // IO-Exceptions:
        CookiesNotFound,
        LoginsNotFound,
        HistoryNotFound,
        BookmarksNotFound,
        WebDataNotFound,
        DownloadsNotFound,
        LocalStateNotFound,
        MozGlueNotFound,
        Nss3NotFound,
        ProfileNotFound,

        // WinApi:
        AddressNotFound,
        FunctionNotFound,

        // Other:
        CouldNotSetProfile,
        ProcessIsNot64Bit,

        NoArgumentsSpecified,
    }
}
