using System;

namespace CockyGrabber
{
    public static partial class Gecko
    {
        public class Bookmark
        {
            public enum Header
            {
                id,
                type,
                fk,
                parent,
                position,
                title,
                keyword_id,
                folder_type,
                dateAdded,
                lastModified,
                guid,
                syncStatus,
                syncChangeCounter,
            }

            public int Id { get; set; }
            public int Type { get; set; }
            public int ForeignKey { get; set; }
            public int Parent { get; set; }
            public int Position { get; set; }
            public string Title { get; set; }
            public int KeywordId { get; set; }
            public string FolderType { get; set; }
            public DateTimeOffset DateAdded { get; set; }
            public DateTimeOffset LastModified { get; set; }
            public string Guid { get; set; }
            public int SyncStatus { get; set; }
            public int SyncChangeCounter { get; set; }

            public string Url { get; set; }

            public override string ToString() => $"Title = '{Title}' | Url = '{Url}'";
        }
    }
}