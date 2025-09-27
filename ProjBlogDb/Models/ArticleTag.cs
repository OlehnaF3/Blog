using System.Text.Json.Serialization;

namespace ProjBlogDb.Models
{
    public class ArticleTag : BaseEntity
    {
        public int ArticleId { get; set; }
        public int TagId { get; set; }

        // Навигационные свойства
        [JsonIgnore]
        public virtual Article Article { get; set; } = null!;
        [JsonIgnore]
        public virtual Tag Tag { get; set; } = null!;
    }
}
