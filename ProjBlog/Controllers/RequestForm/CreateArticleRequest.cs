using System.ComponentModel.DataAnnotations;

namespace ProjBlog.Controllers.RequestForm
{
    public class CreateArticleRequest
    {
        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        public int UserId { get; set; } = 0;

        [Required]
        public string Content { get; set; } = string.Empty;

        public bool IsPublished { get; set; } = true;

        public List<string> Tags { get; set; } = new();
    }
}
