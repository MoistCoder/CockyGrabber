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

            public int Id { get; set; }
            public string Guid { get; set; }
            public string CurrentPath { get; set; }
            public string TargetPath { get; set; }
            public DateTimeOffset StartTime { get; set; }
            public int ReceivedBytes { get; set; }
            public int TotalBytes { get; set; }
            public short State { get; set; }
            public short DangerType { get; set; }
            public short InterruptReason { get; set; }
            public byte[] Hash { get; set; }
            public DateTimeOffset EndTime { get; set; }
            public bool IsOpened { get; set; }
            public DateTimeOffset LastAccessTime { get; set; }
            public int Transient { get; set; }
            public string Referrer { get; set; }
            public string SiteUrl { get; set; }
            public string TabUrl { get; set; }
            public string TabReferrerUrl { get; set; }
            public string HttpMethod { get; set; }
            public string ByExtId { get; set; }
            public string ByExtName { get; set; }
            public string Etag { get; set; }
            public DateTimeOffset LastModified { get; set; }
            public string MimeType { get; set; }
            public string OriginalMimeType { get; set; }
            public string EmbedderDownloadData { get; set; }

            public string Url { get; set; }
            public string Filename { get; set; }

            public override string ToString() => $"Filename = '{Filename}' | Url = '{Url}'";
        }
    }
}