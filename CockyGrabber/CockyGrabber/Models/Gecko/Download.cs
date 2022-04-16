using System;

namespace CockyGrabber
{
    public static partial class Gecko
    {
        public class Download
        {
            public enum Header
            {
                id,
                place_id,
                anno_attribute_id,
                content,
                flags,
                expiration,
                type,
                dateAdded,
                lastModified,
            }

            public int Id { get; set; }
            public int PlaceId { get; set; }
            public int AnnoAttributeId { get; set; }
            public string Content { get; set; }
            public short Flags { get; set; }
            public int Expiration { get; set; }
            public short Type { get; set; }
            public DateTimeOffset DateAdded { get; set; }
            public DateTimeOffset LastModified { get; set; }

            public string Url { get; set; }
            public string Filename { get; set; }
            public short State { get; set; }
            public DateTimeOffset EndTime { get; set; }
            public long FileSize { get; set; }

            public override string ToString() => $"Filename = '{Filename}' | Url = '{Url}'";
        }
    }
}