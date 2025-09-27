using System.ComponentModel.DataAnnotations.Schema;

namespace ProjBlogDb.Models
{
    public abstract class BaseEntity : IEntity
    {
        public int Id { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
