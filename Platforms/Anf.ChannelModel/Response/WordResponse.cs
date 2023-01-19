using Anf.ChannelModel.Entity;
using System;

namespace Anf.ChannelModel
{
    public class WordResponse
    {
        public ulong Id { get; set; }

        public WordType Type { get; set; }

        public string Text { get; set; }

        public ushort Length { get; set; }

        public long CreatorId { get; set; }

        public string CreatorName { get; set; }

        public long? AuthorId { get; set; }

        public string AuthorName { get; set; }

        public ulong LikeCount { get; set; }

        public ulong VisitCount { get; set; }

        public CommitTypes CommitType { get; set; }

        public string From { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
