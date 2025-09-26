using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProjBlog.Models;

namespace ProjBlog.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<User?> GetUserById(int id,CancellationToken cancellation = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default);
        Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
        public Task<EntityEntry<User>?> AddAsync(User user, CancellationToken cancellationToken = default);
    }
}
