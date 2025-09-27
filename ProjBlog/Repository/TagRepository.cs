using Microsoft.EntityFrameworkCore;
using ProjBlogDb.DbContext;
using ProjBlogDb.Models;
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
    }
}
