using System;

namespace CockyGrabber
{
    public static partial class Blink
    {
        public class Cookie
        {
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

            public DateTimeOffset CreationUTC { get; set; }
            public string TopFrameSiteKey { get; set; }
            public string HostKey { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
            public string EncryptedValue { get; set; }
            public string DecryptedValue { get; set; }
            public string Path { get; set; }
            public DateTimeOffset ExpiresUTC { get; set; }
            public bool IsSecure { get; set; }
            public bool IsHttpOnly { get; set; }
            public DateTimeOffset LastAccessUTC { get; set; }
            public bool HasExpires { get; set; }
            public bool IsPersistent { get; set; }
            public short Priority { get; set; }
            public short Samesite { get; set; }
            public short SourceScheme { get; set; }
            public int SourcePort { get; set; }
            public bool IsSameParty { get; set; }

            public override string ToString() => $"HostKey = '{HostKey}' | Name = '{Name}' | DecryptedValue = '{DecryptedValue}'";
        }
    }
}