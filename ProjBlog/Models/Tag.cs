using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProjBlog.Models
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public int ArticleTagsId { get; set; }
        // Навигационные свойства
        public virtual ICollection<ArticleTag> ArticleTags { get; set; } = new List<ArticleTag>();
    }
}
