using ProjBlog.Models;

namespace ProjBlog.Repository
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetByArticleIdAsync(int articleId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Comment>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Comment>> GetChildCommentsAsync(int parentCommentId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Comment>> GetApprovedCommentsAsync(int articleId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Comment>> GetCommentsWithRepliesAsync(int articleId, CancellationToken cancellationToken = default);
    }
}
