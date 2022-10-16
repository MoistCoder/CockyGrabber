using CockyGrabber.Utility;
using System;

namespace CockyGrabber
{
    public static partial class Gecko
    {
        public class Cookie
        {
            public enum SameSiteType
            {
                /// <summary>
                /// Cookies will be sent in all contexts, i.e. in responses to both first-party and cross-site requests.
                /// </summary>
                None = 0,
                /// <summary>
                /// Cookies are not sent on normal cross-site subrequests (for example to load images or frames into a third party site), but are sent when a user is navigating to the origin site (i.e., when following a link).
                /// </summary>
                Lax = 1,
                /// <summary>
                /// Cookies will only be sent in a first-party context and not be sent along with requests initiated by third party websites.
                /// </summary>
                Strict = 2,
            }
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

            /// <summary>
            /// The cookie's unique identifier.
            /// </summary>
            public int Id { get; set; }
            /// <summary>
            /// The cookie's origin attributes.
            /// </summary>
            public string OriginAttributes { get; set; }
            /// <summary>
            /// The cookie's name.
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// The cookie's value.
            /// </summary>
            public string Value { get; set; }
            /// <summary>
            /// The cookie's host. (the domain)
            /// </summary>
            public string Host { get; set; }
            /// <summary>
            /// The cookie's path.
            /// </summary>
            public string Path { get; set; }
            /// <summary>
            /// The cookie's expiry date.
            /// </summary>
            public DateTimeOffset Expiry { get; set; }
            /// <summary>
            /// Time at which the cookie was last accessed.
            /// </summary>
            public DateTimeOffset LastAccessed { get; set; }
            /// <summary>
            /// Time at which the cookie was created.
            /// </summary>
            public DateTimeOffset CreationTime { get; set; }
            /// <summary>
            /// Boolean indicating if the cookie uses a secure connection.
            /// </summary>
            public bool IsSecure { get; set; }
            /// <summary>
            /// Boolean indicating if the cookie works only with HTTP.
            /// </summary>
            public bool IsHttpOnly { get; set; }
            /// <summary>
            /// Boolean indicating if the cookie is a in browser element.
            /// </summary>
            public bool InBrowserElement { get; set; }
            /// <summary>
            /// SameSite determines whether the cookie can be sent along with cross-site requests.
            /// </summary>
            public SameSiteType SameSite { get; set; }
            /// <summary>
            /// I don't fucking know
            /// </summary>
            public short RawSameSite { get; set; }
            /// <summary>
            /// I don't fucking know
            /// </summary>
            public short SchemeMap { get; set; }

            public override string ToString() => $"Host = '{Host}' | Name = '{Name}' | Value = '{Value}'";
            public string ToJson() => Tools.BrowserInformationToJson(this);
        }
    }
}