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

            public int Id { get; set; }
            public string Url { get; set; }
            public string Title { get; set; }
            public string RevHost { get; set; }
            public int VisitCount { get; set; }
            public bool IsHidden { get; set; }
            public bool IsTyped { get; set; }
            public int Frecency { get; set; }
            public DateTimeOffset LastVisitDate { get; set; }
            public string Guid { get; set; }
            public int ForeignCount { get; set; }
            public long UrlHash { get; set; }
            public string Description { get; set; }
            public string PreviewImageUrl { get; set; }
            public int Originid { get; set; }
            public string SiteName { get; set; }

            public override string ToString() => $"Title = '{Title}' | Url = '{Url}'";
        }
    }
}