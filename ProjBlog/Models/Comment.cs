using System.Text.Json.Serialization;

namespace ProjBlog.Models
{
    public class Comment : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public int ArticleId { get; set; }
        public int UserId { get; set; }
        public int? ParentCommentId { get; set; }
        public bool IsApproved { get; set; } = false;
        // Навигационные свойства
        public virtual Article Article { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual Comment? ParentComment { get; set; }
        [JsonIgnore]
        public virtual ICollection<Comment> ChildComments { get; set; } = new List<Comment>();

        // Методы
        public void Approve() => IsApproved = true;
        public void Disapprove() => IsApproved = false;

        public void AddReply(Comment reply)
        {
            reply.ParentCommentId = Id;
            reply.ArticleId = ArticleId;
            ChildComments.Add(reply);
        }
    }
}
