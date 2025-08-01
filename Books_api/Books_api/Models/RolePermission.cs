namespace Books_api.Models
{
    public class RolePermission
    {

    }

    public class AssignRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }

    public class AssignPermission
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
    }

}
