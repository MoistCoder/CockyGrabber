using System;

namespace CockyGrabber
{
    public static partial class Blink
    {
        public class Login
        {
            public enum Header
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

            public string OriginUrl { get; set; }
            public string ActionUrl { get; set; }
            public string UsernameElement { get; set; }
            public string UsernameValue { get; set; }
            public string PasswordElement { get; set; }
            public string PasswordValue { get; set; }
            public string DecryptedPasswordValue { get; set; }
            public string SubmitElement { get; set; }
            public string SignonRealm { get; set; }
            public DateTimeOffset DateCreated { get; set; }
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
            public DateTimeOffset DateLastUsed { get; set; }
            public string MovingBlockedFor { get; set; }
            public DateTimeOffset DatePasswordModified { get; set; }

            public override string ToString() => $"OriginUrl = '{OriginUrl}' | UsernameValue = '{UsernameValue}' | DecryptedPasswordValue = '{DecryptedPasswordValue}'";
        }
    }
}