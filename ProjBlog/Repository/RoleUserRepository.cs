using ProjBlogDb.DbContext;
using ProjBlogDb.Models;

namespace ProjBlog.Repository
{
    public class RoleUserRepository : Repository<RoleUser>,IRoleUserRepository
    {
        public RoleUserRepository(BlogDbContext context) : base(context)
        {
        }
    }
}
