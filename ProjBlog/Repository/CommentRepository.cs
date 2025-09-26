using Microsoft.EntityFrameworkCore;
using ProjBlog.DbContext;
using ProjBlog.Models;

namespace ProjBlog.Repository
{

    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(BlogDbContext context) : base(context) { }

        public async Task<IEnumerable<Comment>> GetByArticleIdAsync(int articleId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(c => c.ArticleId == articleId && c.ParentCommentId == null)
                .Include(c => c.User)
                .Include(c => c.ChildComments)
                    .ThenInclude(cc => cc.User)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Comment>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(c => c.UserId == userId)
                .Include(c => c.Article)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Comment>> GetChildCommentsAsync(int parentCommentId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(c => c.ParentCommentId == parentCommentId)
                .Include(c => c.User)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Comment>> GetApprovedCommentsAsync(int articleId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(c => c.ArticleId == articleId && c.IsApproved)
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Comment>> GetCommentsWithRepliesAsync(int articleId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(c => c.ArticleId == articleId && c.ParentCommentId == null)
                .Include(c => c.User)
                .Include(c => c.ChildComments)
                    .ThenInclude(cc => cc.User)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}
