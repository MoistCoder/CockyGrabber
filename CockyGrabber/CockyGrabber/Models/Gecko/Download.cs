using CockyGrabber.Utility;
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

            /// <summary>
            /// The id of the download.
            /// </summary>
            public int Id { get; set; }
            /// <summary>
            /// The id of the place the download was created for.
            /// </summary>
            public int PlaceId { get; set; }
            /// <summary>
            /// The id of the annotation attribute the download was created for.
            /// </summary>
            public int AnnoAttributeId { get; set; }
            /// <summary>
            /// The download's content.
            /// </summary>
            public string Content { get; set; }
            /// <summary>
            /// The download's flags.
            /// </summary>
            public short Flags { get; set; }
            /// <summary>
            /// The expiration of the download.
            /// </summary>
            public int Expiration { get; set; }
            /// <summary>
            /// The download type.
            /// </summary>
            public short Type { get; set; }
            /// <summary>
            /// Time at which the download was added.
            /// </summary>
            public DateTimeOffset DateAdded { get; set; }
            /// <summary>
            /// Time at which the download was last modified.
            /// </summary>
            public DateTimeOffset LastModified { get; set; }

            /// <summary>
            /// The URL from which the file was downloaded.
            /// </summary>
            public string Url { get; set; }
            /// <summary>
            /// The downloaded file's name.
            /// </summary>
            public string Filename { get; set; }
            /// <summary>
            /// The download's state.
            /// </summary>
            public short State { get; set; }
            /// <summary>
            /// Time at which the download ended.
            /// </summary>
            public DateTimeOffset EndTime { get; set; }
            /// <summary>
            /// Size of the downloaded file.
            /// </summary>
            public long FileSize { get; set; }

            public override string ToString() => $"Filename = '{Filename}' | Url = '{Url}'";
            public string ToJson() => Tools.BrowserInformationToJson(this);
        }
    }
}