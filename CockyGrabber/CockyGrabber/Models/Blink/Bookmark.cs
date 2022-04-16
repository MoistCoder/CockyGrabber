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

            public DateTimeOffset DateAdded { get; set; }
            public string Guid { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Url { get; set; }

            public override string ToString() => $"Name = '{Name}' | Url = '{Url}'";
        }
    }
}