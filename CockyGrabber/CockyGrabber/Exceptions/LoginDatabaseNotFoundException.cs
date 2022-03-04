using System;
using System.IO;

namespace CockyGrabber
{
    [Serializable]
    public class LoginDatabaseNotFoundException : Exception
    {
        public LoginDatabaseNotFoundException()
        { }

        public LoginDatabaseNotFoundException(string path) : base("The Login database could not be found: " + path, new FileNotFoundException())
        { }

        public LoginDatabaseNotFoundException(string path, Exception innerException) : base("The Login database could not be found: " + path, innerException)
        { }
    }
}