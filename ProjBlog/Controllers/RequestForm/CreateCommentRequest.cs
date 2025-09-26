using System.ComponentModel.DataAnnotations;

namespace ProjBlog.Controllers.RequestForm
{
    public class CreateCommentRequest
    {
        [Required]

        public int UserId { get; set; }

        public string Content { get; set; } = string.Empty;

        public int? ParentCommentId { get; set; }
    }
}
