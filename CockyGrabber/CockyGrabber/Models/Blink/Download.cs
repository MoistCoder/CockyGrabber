using CockyGrabber.Utility;
using System;

namespace CockyGrabber
{
    public static partial class Blink
    {
        public class Download
        {
            public enum Header
            {
                id,
                guid,
                current_path,
                target_path,
                start_time,
                received_bytes,
                total_bytes,
                state,
                danger_type,
                interrupt_reason,
                hash,
                end_time,
                opened,
                last_access_time,
                transient,
                referrer,
                site_url,
                tab_url,
                tab_referrer_url,
                http_method,
                by_ext_id,
                by_ext_name,
                etag,
                last_modified,
                mime_type,
                original_mime_type,
                embedder_download_data,
            }

            /// <summary>
            /// The ID of the download.
            /// </summary>
            public int Id { get; set; }
            /// <summary>
            /// The GUID of the download.
            /// </summary>
            public string Guid { get; set; }
            /// <summary>
            /// The current path of the download
            /// </summary>
            public string CurrentPath { get; set; }
            /// <summary>
            /// The target path of the download
            /// </summary>
            public string TargetPath { get; set; }
            /// <summary>
            /// The time at which the download started.
            /// </summary>
            public DateTimeOffset StartTime { get; set; }
            /// <summary>
            /// The number of received bytes.
            /// </summary>
            public int ReceivedBytes { get; set; }
            /// <summary>
            /// The number of total bytes.
            /// </summary>
            public int TotalBytes { get; set; }
            /// <summary>
            /// The download's state.
            /// </summary>
            public short State { get; set; }
            /// <summary>
            /// The download's danger type.
            /// </summary>
            public short DangerType { get; set; }
            /// <summary>
            /// The download's interrupt reason.
            /// </summary>
            public short InterruptReason { get; set; }
            /// <summary>
            /// The download's hash.
            /// </summary>
            public byte[] Hash { get; set; }
            /// <summary>
            /// The time at which the download ended.
            /// </summary>
            public DateTimeOffset EndTime { get; set; }
            /// <summary>
            /// Boolean indicating if the download is opened.
            /// </summary>
            public bool IsOpened { get; set; }
            /// <summary>
            /// The time at which the download was last accessed.
            /// </summary>
            public DateTimeOffset LastAccessTime { get; set; }
            /// <summary>
            /// I don't fucking know.
            /// </summary>
            public int Transient { get; set; }
            /// <summary>
            /// The download's referrer.
            /// </summary>
            public string Referrer { get; set; }
            /// <summary>
            /// The download's site URL.
            /// </summary>
            public string SiteUrl { get; set; }
            /// <summary>
            /// The download's tab URL.
            /// </summary>
            public string TabUrl { get; set; }
            /// <summary>
            /// The download's tab referrer URL.
            /// </summary>
            public string TabReferrerUrl { get; set; }
            /// <summary>
            /// The download's http method.
            /// </summary>
            public string HttpMethod { get; set; }
            /// <summary>
            /// The download's by extension identifier.
            /// </summary>
            public string ByExtId { get; set; }
            /// <summary>
            /// The download's by extension name.
            /// </summary>
            public string ByExtName { get; set; }
            /// <summary>
            /// The download's e-tag.
            /// </summary>
            public string Etag { get; set; }
            /// <summary>
            /// The time at which the download was last modified.
            /// </summary>
            public DateTimeOffset LastModified { get; set; }
            /// <summary>
            /// The download's mime type.
            /// </summary>
            public string MimeType { get; set; }
            /// <summary>
            /// The download's original mime type.
            /// </summary>
            public string OriginalMimeType { get; set; }
            /// <summary>
            /// The download's embedder download data.
            /// </summary>
            public string EmbedderDownloadData { get; set; }

            /// <summary>
            /// The URL from which the file was downloaded.
            /// </summary>
            public string Url { get; set; }
            /// <summary>
            /// The downloaded file's name.
            /// </summary>
            public string Filename { get; set; }

            public override string ToString() => $"Filename = '{Filename}' | Url = '{Url}'";
            public string ToJson() => Tools.BrowserInformationToJson(this);
        }
    }
}