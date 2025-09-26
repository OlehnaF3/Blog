using Microsoft.EntityFrameworkCore;
using ProjBlog.DbContext;
using ProjBlog.Models;
using ProjBlog.Repository;

namespace ProjBlog.Repository
{

    public class TagRepository : Repository<Tag>, ITagRepository
    {
        public TagRepository(BlogDbContext context) : base(context) { }

        public async Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
        }

        public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(t => t.Name == name, cancellationToken);
        }

        public async Task<IEnumerable<Tag>> GetPopularTagsAsync(int count, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Select(t => new
                {
                    Tag = t,
                    ArticleCount = t.ArticleTags.Count
                })
                .OrderByDescending(x => x.ArticleCount)
                .Select(x => x.Tag)
                .Take(count)
                .ToListAsync(cancellationToken);
        }
    }
}
