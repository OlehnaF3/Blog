using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace ProjBlogDb.Models
{
    public class User : BaseEntity
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;
        [Column("is_active")]
        public bool IsActive { get; set; } = true;
        [Column("RoleUserId")]
        public int RoleUserId { get; set; }

        // Навигационные свойства
        [JsonIgnore]
        public virtual ICollection<RoleUser> RoleUser { get; set; } = new List<RoleUser>();
        [JsonIgnore]
        public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
        [JsonIgnore]
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        // Методы
        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}
