using System;

namespace CockyGrabber
{
    public class GrabberException : Exception
    {
        public GrabberError Error { get; set; }
        public GrabberException(GrabberError e) : base()
        {
            Error = e;
        }

        public GrabberException(GrabberError e, string msg) : base(msg)
        {
            Error = e;
        }

        public GrabberException(GrabberError e, string msg, Exception innerException) : base(msg, innerException)
        {
            Error = e;
        }
    }
}
