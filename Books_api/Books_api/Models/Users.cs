using System.ComponentModel.DataAnnotations;

namespace Books_api.Models
{
    public class UsersParameters
    {
        public int? UserId { get; set; }
        [Required]
        [MaxLength(30)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(50)]
        public string FullName { get; set; }
        [Required]
        public string Password { get; set; }
        public int? RoleType { get; set; }
        public bool Status { get; set; }
    }
    
    public class Users
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public int RoleType { get; set; }
        public bool Status { get; set; }
    }

    public class UsersRoleSetParameters
    {
        [Required]
        public int RoleType { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public bool Status { get; set; }
    }

    public class ListUsersParameter
    {
        public int? RoleType { get; set; }
        public bool? Status { get; set; }
    }
}
