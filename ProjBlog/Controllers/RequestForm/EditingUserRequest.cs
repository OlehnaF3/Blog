using System.ComponentModel.DataAnnotations.Schema;

namespace ProjBlog.Controllers
{
    public class EditingUserRequest
    {
        public int UserId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public string Role { get; set; } = string.Empty;

    }
}
