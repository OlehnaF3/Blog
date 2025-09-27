using Microsoft.EntityFrameworkCore;
using ProjBlogDb.DbContext;
using ProjBlogDb.Models;

namespace ProjBlog.Repository
{

    public class ArticleRepository : Repository<Article>, IArticleRepository
    {
        public ArticleRepository(BlogDbContext context) : base(context) { }

        public async Task<IEnumerable<Article>> GetByAuthorIdAsync(int authorId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(a => a.AuthorId == authorId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Article>> GetPublishedArticlesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(a => a.IsPublished)
                .Include(a => a.Author)
                .Include(a => a.ArticleTags)
                    .ThenInclude(at => at.Tag)
                .OrderByDescending(a => a.PublishedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Article>> GetArticlesWithTagsAndAuthorAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(a => a.Author)
                .Include(a => a.ArticleTags)
                    .ThenInclude(at => at.Tag)
                .Include(a => a.Comments)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Article>> GetByTagIdAsync(int tagId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(a => a.ArticleTags.Any(at => at.TagId == tagId) && a.IsPublished)
                .Include(a => a.Author)
                .OrderByDescending(a => a.PublishedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<Article?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(a => a.Author)
                .Include(a => a.ArticleTags)
                    .ThenInclude(at => at.Tag)
                .Include(a => a.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }
    }
}
