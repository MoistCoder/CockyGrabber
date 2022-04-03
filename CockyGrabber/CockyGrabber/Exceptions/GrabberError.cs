using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CockyGrabber
{
    public enum GrabberError
    {
        UnknownError,

        // IO-Exceptions:
        CookiesNotFound,
        LoginsNotFound,
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
