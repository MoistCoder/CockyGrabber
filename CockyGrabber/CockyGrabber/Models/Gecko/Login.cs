using CockyGrabber.Utility;
using System;

namespace CockyGrabber
{
    public static partial class Gecko
    {
        public class Login
        {
            public enum Header
            {
                id,
                hostname,
                httpRealm,
                formSubmitURL,
                usernameField,
                passwordField,
                encryptedUsername,
                encryptedPassword,
                guid,
                encType,
                timeCreated,
                timeLastUsed,
                timePasswordChanged,
                timesUsed,
            }

            /// <summary>
            /// The id of the login.
            /// </summary>
            public int Id { get; set; }
            /// <summary>
            /// The hostname of the login. (The host the login is for)
            /// </summary>
            public string Hostname { get; set; }
            /// <summary>
            /// The HttpRealm of the login.
            /// </summary>
            public string HttpRealm { get; set; }
            /// <summary>
            /// The form submit URL of the login.
            /// </summary>
            public string FormSubmitURL { get; set; }
            /// <summary>
            /// The username field of the login.
            /// </summary>
            public string UsernameField { get; set; }
            /// <summary>
            /// The password field of the login.
            /// </summary>
            public string PasswordField { get; set; }
            /// <summary>
            /// The encrypted username.
            /// </summary>
            public string EncryptedUsername { get; set; }
            /// <summary>
            /// The decrypted username.
            /// </summary>
            public string DecryptedUsername { get; set; }
            /// <summary>
            /// The encrypted password.
            /// </summary>
            public string EncryptedPassword { get; set; }
            /// <summary>
            /// The decrypted password.
            /// </summary>
            public string DecryptedPassword { get; set; }
            /// <summary>
            /// The login's guid.
            /// </summary>
            public string Guid { get; set; }
            /// <summary>
            /// The enc type of the login.
            /// </summary>
            public short EncType { get; set; }
            /// <summary>
            /// Time at which the login was created.
            /// </summary>
            public DateTimeOffset TimeCreated { get; set; }
            /// <summary>
            /// Time at which the login was last used.
            /// </summary>
            public DateTimeOffset TimeLastUsed { get; set; }
            /// <summary>
            /// Time at which the login's password was last changed.
            /// </summary>
            public DateTimeOffset TimePasswordChanged { get; set; }
            /// <summary>
            /// Number of times used.
            /// </summary>
            public uint TimesUsed { get; set; }

            public override string ToString() => $"Hostname = '{Hostname}' | DecryptedUsername = '{DecryptedUsername}' | DecryptedPassword = '{DecryptedPassword}'";
            public string ToJson() => Tools.BrowserInformationToJson(this);
        }
    }
}