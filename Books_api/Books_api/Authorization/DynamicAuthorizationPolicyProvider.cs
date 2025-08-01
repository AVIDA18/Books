using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Books_api.Authorization
{
    public class DynamicAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

        public DynamicAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return _fallbackPolicyProvider.GetDefaultPolicyAsync();
        }

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return _fallbackPolicyProvider.GetFallbackPolicyAsync();
        }

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            // Dynamically create a policy with the name used in [Authorize(Policy = "...")]
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new DynamicPermissionRequirement())
                .Build();

            return Task.FromResult<AuthorizationPolicy?>(policy);
        }
    }
}
