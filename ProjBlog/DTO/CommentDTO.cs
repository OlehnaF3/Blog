namespace ProjBlog.DTO
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public UserDto Author { get; set; } = null!;
        public DateTime CreatedAt { get; init; }
        public bool IsApproved { get; set; }
        public int? ParentCommentId { get; set; }
        public List<CommentDto> Replies { get; set; } = new();
    }
}
