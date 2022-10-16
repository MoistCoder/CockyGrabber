using CockyGrabber.Utility;
using System;

namespace CockyGrabber
{
    public static partial class Blink
    {
        public class Site
        {
            public enum Header
            {
                id,
                url,
                title,
                visit_count,
                typed_count,
                last_visit_time,
                hidden,
            }

            /// <summary>
            /// The identifer of the site entry.
            /// </summary>
            public int Id { get; set; }
            /// <summary>
            /// The URL of the site.
            /// </summary>
            public string Url { get; set; }
            /// <summary>
            /// The title of the site.
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// The visit count of the site.
            /// </summary>
            public int VisitCount { get; set; }
            /// <summary>
            /// The number of times that the site was typed in the search bar.
            /// </summary>
            public int TypedCount { get; set; }
            /// <summary>
            /// The time at which the site was last visited.
            /// </summary>
            public DateTimeOffset LastVisitTime { get; set; }
            /// <summary>
            /// Boolean indicating if the site entry is hidden in the history tab.
            /// </summary>
            public bool IsHidden { get; set; }
            /// <summary>
            /// The number of visits of the site.
            /// </summary>

            public override string ToString() => $"Title = '{Title}' | Url = '{Url}'";
            public string ToJson() => Tools.BrowserInformationToJson(this);
        }
    }
}