using CockyGrabber.Utility;
using System;

namespace CockyGrabber
{
    public static partial class Blink
    {
        public class Bookmark
        {
            public enum Header
            {
                date_added,
                guid,
                id,
                name,
                type,
                url,
            }

            /// <summary>
            /// The date the bookmark was added.
            /// </summary>
            public DateTimeOffset DateAdded { get; set; }
            /// <summary>
            /// The guid of the bookmark.
            /// </summary>
            public string Guid { get; set; }
            /// <summary>
            /// The identifier of the bookmark.
            /// </summary>
            public int Id { get; set; }
            /// <summary>
            /// The bookmark's name.
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// The bookmark type.
            /// </summary>
            public string Type { get; set; }
            /// <summary>
            /// The URL of the bookamrk.
            /// </summary>
            public string Url { get; set; }

            public override string ToString() => $"Name = '{Name}' | Url = '{Url}'";
            public string ToJson() => Tools.BrowserInformationToJson(this);
        }
    }
}