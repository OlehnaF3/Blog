using System.Text.Json.Serialization;

namespace ProjBlogDb.Models
{
    public class Role : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<RoleUser> RoleUser { get; set; } = new List<RoleUser>();
    }
}
