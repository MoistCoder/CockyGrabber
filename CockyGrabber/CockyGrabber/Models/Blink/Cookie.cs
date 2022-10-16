using CockyGrabber.Utility;
using System;

namespace CockyGrabber
{
    public static partial class Blink
    {
        public class Cookie
        {
            public enum SameSiteType
            {
                /// <summary>
                /// 'unspecified' corresponds to a cookie set without the SameSite attribute.
                /// </summary>
                Unspecified = -1,
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
                creation_utc,
                top_frame_site_key,
                host_key,
                name,
                value,
                encrypted_value,
                path,
                expires_utc,
                is_secure,
                is_httponly,
                last_access_utc,
                has_expires,
                is_persistent,
                priority,
                samesite,
                source_scheme,
                source_port,
                is_same_party,
            }

            /// <summary>
            /// The creation time of the cookie.
            /// </summary>
            public DateTimeOffset CreationUTC { get; set; }
            /// <summary>
            /// The top frame site key of the cookie.
            /// </summary>
            public string TopFrameSiteKey { get; set; }
            /// <summary>
            /// The host key of the cookie. (the hostname/domain)
            /// </summary>
            public string HostKey { get; set; }
            /// <summary>
            /// The name of the cookie.
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// The value of the cookie.
            /// <para>This is not the decrypted value!</para>
            /// </summary>
            public string Value { get; set; }
            /// <summary>
            /// The encrypted value of the cookie.
            /// </summary>
            public byte[] EncryptedValue { get; set; }
            /// <summary>
            /// The decrypted value of the cookie.
            /// </summary>
            public string DecryptedValue { get; set; }
            /// <summary>
            /// The path of the cookie.
            /// </summary>
            public string Path { get; set; }
            /// <summary>
            /// The cookie's expiry date.
            /// </summary>
            public DateTimeOffset ExpiresUTC { get; set; }
            /// <summary>
            /// Boolean indicating if the cookie uses a secure connection.
            /// </summary>
            public bool IsSecure { get; set; }
            /// <summary>
            /// Boolean indicating if the cookie is a in browser element.
            /// </summary>
            public bool IsHttpOnly { get; set; }
            /// <summary>
            /// Time at which the cookie was last accessed.
            /// </summary>
            public DateTimeOffset LastAccessUTC { get; set; }
            /// <summary>
            /// Boolean indicating if the cookie does expire.
            /// </summary>
            public bool HasExpires { get; set; }
            /// <summary>
            /// Boolean indicating if the cookie is persistent.
            /// </summary>
            public bool IsPersistent { get; set; }
            /// <summary>
            /// The priority of the cookie.
            /// </summary>
            public short Priority { get; set; }
            /// <summary>
            /// SameSite determines whether the cookie can be sent along with cross-site requests.
            /// </summary>
            public SameSiteType Samesite { get; set; }
            /// <summary>
            /// I don't fucking know
            /// </summary>
            public short SourceScheme { get; set; }
            /// <summary>
            /// The port of the cookie's host. 
            /// </summary>
            public int SourcePort { get; set; }
            /// <summary>
            /// I don't fucking know
            /// </summary>
            public bool IsSameParty { get; set; }

            public override string ToString() => $"HostKey = '{HostKey}' | Name = '{Name}' | DecryptedValue = '{DecryptedValue}'";
            public string ToJson() => Tools.BrowserInformationToJson(this);
        }
    }
}