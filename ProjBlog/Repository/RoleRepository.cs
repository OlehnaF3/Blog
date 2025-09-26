using Microsoft.EntityFrameworkCore;
using ProjBlog.DbContext;
using ProjBlog.Models;

namespace ProjBlog.Repository
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(BlogDbContext context) : base(context)
        {
            
        }

        public async Task<Role> GetRoleByRolename(string rolename, CancellationToken cancellation = default)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.Name == rolename, cancellation);
        }

        public Task<bool> RoleExitst(string rolename, CancellationToken cancellation = default)
        {
          return _dbSet.AnyAsync(r => r.Name == rolename, cancellation);

        }

    }
}
