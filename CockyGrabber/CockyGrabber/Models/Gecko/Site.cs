using CockyGrabber.Utility;
using System;

namespace CockyGrabber
{
    public static partial class Gecko
    {
        public class Site
        {
            public enum Header
            {
                id,
                url,
                title,
                rev_host,
                visit_count,
                hidden,
                typed,
                frecency,
                last_visit_date,
                guid,
                foreign_count,
                url_hash,
                description,
                preview_image_url,
                origin_id,
                site_name,
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
            /// The reverse host of the site.
            /// </summary>
            public string RevHost { get; set; }
            /// <summary>
            /// The visit count of the site.
            /// </summary>
            public int VisitCount { get; set; }
            /// <summary>
            /// Boolean indicating if the site entry is hidden in the history tab.
            /// </summary>
            public bool IsHidden { get; set; }
            /// <summary>
            /// Boolean indicating if the site is typed.
            /// </summary>
            public bool IsTyped { get; set; }
            /// <summary>
            /// The site's frecency.
            /// </summary>
            public int Frecency { get; set; }
            /// <summary>
            /// Time & Date at which the site was last visited.
            /// </summary>
            public DateTimeOffset LastVisitDate { get; set; }
            /// <summary>
            /// The site entries guid.
            /// </summary>
            public string Guid { get; set; }
            /// <summary>
            /// The site's foreign count.
            /// </summary>
            public int ForeignCount { get; set; }
            /// <summary>
            /// The site's URL hash.
            /// </summary>
            public long UrlHash { get; set; }
            /// <summary>
            /// The site's description.
            /// </summary>
            public string Description { get; set; }
            /// <summary>
            /// The site's preview image URL.
            /// </summary>
            public string PreviewImageUrl { get; set; }
            /// <summary>
            /// The site's origin identifier.
            /// </summary>
            public int Originid { get; set; }
            /// <summary>
            /// The site's name.
            /// </summary>
            public string SiteName { get; set; }

            public override string ToString() => $"Title = '{Title}' | Url = '{Url}'";
            public string ToJson() => Tools.BrowserInformationToJson(this);
        }
    }
}