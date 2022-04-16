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

            public int Id { get; set; }
            public string Url { get; set; }
            public string Title { get; set; }
            public int VisitCount { get; set; }
            public int TypedCount { get; set; }
            public DateTimeOffset LastVisitTime { get; set; }
            public bool IsHidden { get; set; }

            public override string ToString() => $"Title = '{Title}' | Url = '{Url}'";
        }
    }
}