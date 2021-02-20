using System.Text.Json.Serialization;

namespace Kw.Comic.Uwp.Managers
{
    public class BriefRemarkEntity
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("hitokoto")]
        public string Hitokoto { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("from")]
        public string From { get; set; }
        [JsonPropertyName("from_who")]
        public string Who { get; set; }
        [JsonPropertyName("creator")]
        public string Creator { get; set; }
        [JsonPropertyName("creator_uid")]
        public int CreatorId { get; set; }
        [JsonPropertyName("reviewer")]
        public int Reviewer { get; set; }
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }
        [JsonPropertyName("commit_from")]
        public string CommitFrom { get; set; }
        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }
        [JsonPropertyName("length")]
        public int Length { get; set; }
    }
}
