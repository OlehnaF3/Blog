using ProjBlog.Models;

namespace ProjBlog.Repository
{
    public interface IArticleRepository : IRepository<Article>
    {
        Task<IEnumerable<Article>> GetByAuthorIdAsync(int authorId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Article>> GetPublishedArticlesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Article>> GetArticlesWithTagsAndAuthorAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Article>> GetByTagIdAsync(int tagId, CancellationToken cancellationToken = default);
        Task<Article?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    }
}
