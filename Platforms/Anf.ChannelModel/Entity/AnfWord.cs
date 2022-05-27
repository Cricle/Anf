using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Anf.ChannelModel.Entity
{
    public class AnfWord : IdentityDbEntityBase
    {
        [Required]
        public WordType Type { get; set; }

        [Required]
        [MaxLength(512)]
        public string Text { get; set; }

        [Required]
        public ushort Length { get; set; }

        [Required]
        public CommitTypes CommitType { get; set; }

        [Required]
        public ulong LikeCount { get; set; }

        [Required]
        public ulong VisitCount { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        [Required]
        [ForeignKey(nameof(Creator))]
        public long CreatorId { get; set; }

        [ForeignKey(nameof(Author))]
        public long? AuthorId { get; set; }

        [MaxLength(128)]
        public string From { get; set; }

        public virtual AnfUser Creator { get; set; }

        public virtual AnfUser Author { get; set; }

        [NotMapped]
        public IEnumerable<AnfWordLike> Likes { get; set; }

        [NotMapped]
        public IEnumerable<AnfWordVisit> Visits { get; set; }
    }
}
