using ProjBlog.Models;
using ProjBlog.Repository;

namespace ProjBlog.Repository
{
    public interface ITagRepository : IRepository<Tag>
    {
        Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);
        Task<IEnumerable<Tag>> GetPopularTagsAsync(int count, CancellationToken cancellationToken = default);
    }
}
