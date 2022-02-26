namespace CockyGrabber
{
    public static class Chromium
    {
        public class Cookie
        {
            public long CreationUTC { get; set; }
            public string TopFrameSiteKey { get; set; }
            public string HostKey { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
            public string EncryptedValue { get; set; }
            public string Path { get; set; }
            public long ExpiresUTC { get; set; }
            public bool IsSecure { get; set; }
            public bool IsHttpOnly { get; set; }
            public long LastAccessUTC { get; set; }
            public bool HasExpires { get; set; }
            public bool IsPersistent { get; set; }
            public short Priority { get; set; }
            public short Samesite { get; set; }
            public short SourceScheme { get; set; }
            public int SourcePort { get; set; }
            public bool IsSameParty { get; set; }
        }
        public class Login
        {
            public string OriginUrl { get; set; }
            public string ActionUrl { get; set; }
            public string UsernameElement { get; set; }
            public string UsernameValue { get; set; }
            public string PasswordElement { get; set; }
            public string PasswordValue { get; set; }
            public string SubmitElement { get; set; }
            public string SignonRealm { get; set; }
            public long DateCreated { get; set; }
            public bool IsBlacklistedByUser { get; set; }
            public int Scheme { get; set; }
            public int PasswordType { get; set; }
            public int TimesUsed { get; set; }
            public string FormData { get; set; }
            public string DisplayName { get; set; }
            public string IconUrl { get; set; }
            public string FederationUrl { get; set; }
            public int SkipZeroClick { get; set; }
            public int GenerationUploadStatus { get; set; }
            public string PossibleUsernamePairs { get; set; }
            public int Id { get; set; }
            public long DateLastUsed { get; set; }
            public string MovingBlockedFor { get; set; }
            public long DatePasswordModified { get; set; }
        }
        public enum CookieHeader
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
        public enum LoginHeader
        {
            origin_url,
            action_url,
            username_element,
            username_value,
            password_element,
            password_value,
            submit_element,
            signon_realm,
            date_created,
            blacklisted_by_user,
            scheme,
            password_type,
            times_used,
            form_data,
            display_name,
            icon_url,
            federation_url,
            skip_zero_click,
            generation_upload_status,
            possible_username_pairs,
            id,
            date_last_used,
            moving_blocked_for,
            date_password_modified
        }
    }
}