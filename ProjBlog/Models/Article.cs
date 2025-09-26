using System.Text.Json.Serialization;

namespace ProjBlog.Models
{
    public class Article : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public virtual User Author { get; set; } = null!;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsPublished { get; set; } = false;
        public DateTime? PublishedAt { get; set; }

        public int CommentsId { get; set; }

        public int ArticleTagId { get; set; }

        // Навигационные свойства
        [JsonIgnore]
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        [JsonIgnore]
        public virtual ICollection<ArticleTag> ArticleTags { get; set; } = new List<ArticleTag>();


        // Методы
        public void Publish()
        {
            IsPublished = true;
            PublishedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Unpublish()
        {
            IsPublished = false;
            PublishedAt = null;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddTag(Tag tag)
        {
            if (!ArticleTags.Any(at => at.TagId == tag.Id))
            {
                ArticleTags.Add(new ArticleTag { ArticleId = Id, TagId = tag.Id });
            }
        }

        public void RemoveTag(Tag tag)
        {
            var articleTag = ArticleTags.FirstOrDefault(at => at.TagId == tag.Id);
            if (articleTag != null)
            {
                ArticleTags.Remove(articleTag);
            }
        }
    }
}
