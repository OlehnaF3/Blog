namespace ProjBlog.DTO
{
    public class ArticleDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public UserDto Author { get; set; } = null!;
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? PublishedAt { get; set; }
        public List<string> Tags { get; set; } = new();
        public int CommentsCount { get; set; }
    }
}
