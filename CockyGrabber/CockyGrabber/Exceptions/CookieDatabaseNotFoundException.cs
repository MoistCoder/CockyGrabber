using System;
using System.IO;

namespace CockyGrabber
{
    [Serializable]
    public class CookieDatabaseNotFoundException : Exception
    {
        public CookieDatabaseNotFoundException()
        { }

        public CookieDatabaseNotFoundException(string path) : base("The Cookie database could not be found: " + path, new FileNotFoundException())
        { }

        public CookieDatabaseNotFoundException(string path, Exception innerException) : base("The Cookie database could not be found: " + path, innerException)
        { }
    }
}