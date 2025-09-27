using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProjBlogDb.DbContext;
using ProjBlogDb.Models;
namespace ProjBlog.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(BlogDbContext context) : base(context) { }

        public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(u => u.Username == username, cancellationToken);
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<User?> GetUserById(int id, CancellationToken cancellation = default)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Id == id, cancellation);
        }

        async Task<EntityEntry<User>?> IUserRepository.AddAsync(User user, CancellationToken cancellationToken)
        {
            return await _dbSet.AddAsync(user, cancellationToken);
        }
    }
}
