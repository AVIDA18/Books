using Books_api.Data;
using Books_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Books_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AdminClass _admin;

        public AdminController(AdminClass admin)
        {
            _admin = admin;
        }

        [HttpPost("assign-role")]
        public IActionResult AssignRole([FromBody] AssignRole dto)
        {
            _admin.AssignRoleToUserAsync(dto.UserId, dto.RoleId);
            return Ok("Role assigned.");
        }

        [HttpPost("assign-permission")]
        public IActionResult AssignPermission([FromBody] AssignPermission dto)
        {
            _admin.AssignPermissionToRoleAsync(dto.RoleId, dto.PermissionId);
            return Ok("Permission assigned.");
        }
    }
}
