using System.ComponentModel.DataAnnotations;

namespace ProjBlog.Controllers
{
    public class EditingCommentRequest
    {

        [Required]
        public int CommentId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public string Content { get; set; } = string.Empty;

        public int? ParentCommentId { get; set; }
    }
}
