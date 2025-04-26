using System.ComponentModel.DataAnnotations;

namespace Books_api.Models
{
    public class LoginParameters
    {
        [Required]
        [MaxLength(30)]
        public required string UserName { get; set; }
        [Required]
        public required string Password { get; set; }
    }

    public class ListUserData
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int RoleType { get; set; }
        public bool Status { get; set; }
    }

    public class ChangePassword
    {
        public int? UserId { get; set; }
        public string Password { get; set; }
    }
}
