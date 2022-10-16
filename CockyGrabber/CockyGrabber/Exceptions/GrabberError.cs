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
        DownloadsNotFound,
        FormHistoryNotFound,
        CreditCardsNotFound,
        LocalStateNotFound,
        MozGlueNotFound,
        Nss3NotFound,
        Nss3CouldNotBeLoaded,
        ProfilesNotFound,

        // WinApi:
        AddressNotFound,
        FunctionNotFound,

        // Other:
        CouldNotSetProfile,
        ProcessIsNot64Bit,

        NoArgumentsSpecified,
    }
}
