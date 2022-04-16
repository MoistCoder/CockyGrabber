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

            public int Id { get; set; }
            public string Hostname { get; set; }
            public string HttpRealm { get; set; }
            public string FormSubmitURL { get; set; }
            public string UsernameField { get; set; }
            public string PasswordField { get; set; }
            public string EncryptedUsername { get; set; }
            public string DecryptedUsername { get; set; }
            public string EncryptedPassword { get; set; }
            public string DecryptedPassword { get; set; }
            public string Guid { get; set; }
            public short EncType { get; set; }
            public DateTimeOffset TimeCreated { get; set; }
            public DateTimeOffset TimeLastUsed { get; set; }
            public DateTimeOffset TimePasswordChanged { get; set; }
            public uint TimesUsed { get; set; }

            public override string ToString() => $"Hostname = '{Hostname}' | DecryptedUsername = '{DecryptedUsername}' | DecryptedPassword = '{DecryptedPassword}'";
        }
    }
}