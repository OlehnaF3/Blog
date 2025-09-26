
using System.ComponentModel.DataAnnotations;

namespace ProjBlog.Controllers
{
    public class EditingTagRequest
    {
        [Required]
        public int TagId { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
