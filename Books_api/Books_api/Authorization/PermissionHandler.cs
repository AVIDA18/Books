using Books_api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Security.Claims;

namespace Books_api.Authorization
{
    public class PermissionHandler : AuthorizationHandler<DynamicPermissionRequirement>
    {
        private readonly AdminClass _admin;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionHandler(AdminClass admin, IHttpContextAccessor accessor)
        {
            _admin = admin;
            _httpContextAccessor = accessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, DynamicPermissionRequirement requirement)
        {
            var userIdStr = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return;

            if (!int.TryParse(userIdStr, out int userId)) return;

            // Get endpoint metadata (includes route + Authorize attributes)
            var endpoint = _httpContextAccessor.HttpContext?.GetEndpoint();
            var authorizeAttr = endpoint?.Metadata.GetMetadata<AuthorizeAttribute>();

            var permissionName = authorizeAttr?.Policy;

            if (string.IsNullOrEmpty(permissionName)) return;

            // Check DB for this permission via stored procedure
            var hasPermission = await _admin.UserHasPermissionAsync(userId, permissionName);

            if (hasPermission)
            {
                context.Succeed(requirement);
            }
        }
    }
}
