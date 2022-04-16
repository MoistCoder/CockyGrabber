using System;

namespace CockyGrabber
{
    public static partial class Gecko
    {
        public class Cookie
        {
            public enum Header
            {
                id,
                originAttributes,
                name,
                value,
                host,
                path,
                expiry,
                lastAccessed,
                creationTime,
                isSecure,
                isHttpOnly,
                inBrowserElement,
                sameSite,
                rawSameSite,
                schemeMap,
            }

            public int Id { get; set; }
            public string OriginAttributes { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
            public string Host { get; set; }
            public string Path { get; set; }
            public DateTimeOffset Expiry { get; set; }
            public DateTimeOffset LastAccessed { get; set; }
            public DateTimeOffset CreationTime { get; set; }
            public bool IsSecure { get; set; }
            public bool IsHttpOnly { get; set; }
            public bool InBrowserElement { get; set; }
            public short SameSite { get; set; }
            public short RawSameSite { get; set; }
            public short SchemeMap { get; set; }

            public override string ToString() => $"Host = '{Host}' | Name = '{Name}' | Value = '{Value}'";
        }
    }
}