using System;
using System.IO;

namespace CockyGrabber
{
    [Serializable]
    public class LocalStateNotFoundException : Exception
    {
        public LocalStateNotFoundException()
        { }

        public LocalStateNotFoundException(string path) : base("The Key for decryption (Local State) could not be found: " + path, new FileNotFoundException())
        { }

        public LocalStateNotFoundException(string path, Exception innerException) : base("The Key for decryption (Local State) could not be found: " + path, innerException)
        { }
    }
}