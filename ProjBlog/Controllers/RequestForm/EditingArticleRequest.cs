using System.ComponentModel.DataAnnotations;

namespace ProjBlog.Controllers
{
    public class EditingArticleRequest
    {
        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public int ArticleId { get; set; }

        public int UserId { get; set; }

        public bool IsPublished { get; set; }

        public List<string> Tags { get; set; } = new();
    }
}
