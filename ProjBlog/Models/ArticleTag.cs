using System.Text.Json.Serialization;

namespace ProjBlog.Models
{
    public class ArticleTag : BaseEntity
    {
        public int ArticleId { get; set; }
        public int TagId { get; set; }

        // Навигационные свойства
        public virtual Article Article { get; set; } = null!;
        [JsonIgnore]
        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}
