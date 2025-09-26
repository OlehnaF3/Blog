using System.Text.Json.Serialization;

namespace ProjBlog.Models
{ 
    public class RoleUser : BaseEntity
    {
        
        public int RolesId { get; set; }

        public Role Role { get; set; } = null!;

        public int UserId { get; set; }

        public User User { get; set; } = null!;
    }
}
