using CockyGrabber.Utility;
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

            /// <summary>
            /// The Origin URL of the login. (The host the login is for)
            /// </summary>
            public string OriginUrl { get; set; }
            /// <summary>
            /// The Action URL of the login.
            /// </summary>
            public string ActionUrl { get; set; }
            /// <summary>
            /// The Username Element of the login.
            /// </summary>
            public string UsernameElement { get; set; }
            /// <summary>
            /// The Username Value of the login.
            /// </summary>
            public string UsernameValue { get; set; }
            /// <summary>
            /// The Password Element of the login.
            /// </summary>
            public string PasswordElement { get; set; }
            /// <summary>
            /// The Password Value of the login.
            /// </summary>
            public byte[] PasswordValue { get; set; }
            /// <summary>
            /// The decrypted password value of the login.
            /// </summary>
            public string DecryptedPasswordValue { get; set; }
            /// <summary>
            /// The login's SubmitElement value.
            /// </summary>
            public string SubmitElement { get; set; }
            /// <summary>
            /// The login's SignonRealm value.
            /// </summary>
            public string SignonRealm { get; set; }
            /// <summary>
            /// Time at which the cookie was created.
            /// </summary>
            public DateTimeOffset DateCreated { get; set; }
            /// <summary>
            /// Boolean indicating if the login is blacklisted by the user.
            /// </summary>
            public bool IsBlacklistedByUser { get; set; }
            /// <summary>
            /// I don't fucking know
            /// </summary>
            public int Scheme { get; set; }
            /// <summary>
            /// The password type of the login.
            /// </summary>
            public int PasswordType { get; set; }
            /// <summary>
            /// The number of uses of the login.
            /// </summary>
            public int TimesUsed { get; set; }
            /// <summary>
            /// The login's FormData value.
            /// </summary>
            public string FormData { get; set; }
            /// <summary>
            /// The login's display name.
            /// </summary>
            public string DisplayName { get; set; }
            /// <summary>
            /// The Icon URL of the login.
            /// </summary>
            public string IconUrl { get; set; }
            /// <summary>
            /// The login's federation URL.
            /// </summary>
            public string FederationUrl { get; set; }
            /// <summary>
            /// I don't fucking know.
            /// </summary>
            public int SkipZeroClick { get; set; }
            /// <summary>
            /// I don't fucking know.
            /// </summary>
            public int GenerationUploadStatus { get; set; }
            /// <summary>
            /// Possible username pairs of the login.
            /// </summary>
            public string PossibleUsernamePairs { get; set; }
            /// <summary>
            /// The login's unique identifier.
            /// </summary>
            public int Id { get; set; }
            /// <summary>
            /// Time at which the cookie was last used.
            /// </summary>
            public DateTimeOffset DateLastUsed { get; set; }
            /// <summary>
            /// I don't fucking know.
            /// </summary>
            public string MovingBlockedFor { get; set; }
            /// <summary>
            /// Time at which the cookie was last modified.
            /// </summary>
            public DateTimeOffset DatePasswordModified { get; set; }

            public override string ToString() => $"OriginUrl = '{OriginUrl}' | UsernameValue = '{UsernameValue}' | DecryptedPasswordValue = '{DecryptedPasswordValue}'";
            public string ToJson() => Tools.BrowserInformationToJson(this);
        }
    }
}