using ProjBlog.DbContext;
using ProjBlog.Models;

namespace ProjBlog.Repository
{
    public class RoleUserRepository : Repository<RoleUser>,IRoleUserRepository
    {
        public RoleUserRepository(BlogDbContext context) : base(context)
        {
        }
    }
}
