using ProjBlogDb.Models;

namespace ProjBlog.Repository
{
    public interface IRoleRepository  : IRepository<Role>
    {
        Task<bool> RoleExitst(string rolename,CancellationToken cancellation = default);
        Task<Role?> GetRoleByRolename(string rolename,CancellationToken cancellation = default);
    }
}
