using CockyGrabber.Utility;
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

            /// <summary>
            /// The identifier of the bookmark.
            /// </summary>
            public int Id { get; set; }
            /// <summary>
            /// The bookmark type.
            /// </summary>
            public int Type { get; set; }
            /// <summary>
            /// The foreign key of the bookmark.
            /// </summary>
            public int ForeignKey { get; set; }
            /// <summary>
            /// The parent the bookmark.
            /// </summary>
            public int Parent { get; set; }
            /// <summary>
            /// The position the bookmark.
            /// </summary>
            public int Position { get; set; }
            /// <summary>
            /// The title the bookmark.
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// The keyword id the bookmark.
            /// </summary>
            public int KeywordId { get; set; }
            /// <summary>
            /// The foldertype the bookmark.
            /// </summary>
            public string FolderType { get; set; }
            /// <summary>
            /// The time at which the bookmark was added.
            /// </summary>
            public DateTimeOffset DateAdded { get; set; }
            /// <summary>
            /// The time at which the bookmark was last modified.
            /// </summary>
            public DateTimeOffset LastModified { get; set; }
            /// <summary>
            /// The guid of the bookmark.
            /// </summary>
            public string Guid { get; set; }
            /// <summary>
            /// The synchronization status of the bookmark.
            /// </summary>
            public int SyncStatus { get; set; }
            /// <summary>
            /// The number of time the synchronization was changed.
            /// </summary>
            public int SyncChangeCounter { get; set; }

            /// <summary>
            /// The URL of the bookmark.
            /// </summary>
            public string Url { get; set; }

            public override string ToString() => $"Title = '{Title}' | Url = '{Url}'";
            public string ToJson() => Tools.BrowserInformationToJson(this);
        }
    }
}