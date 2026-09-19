using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Lumen.Services.UserServices
{
    // Lumen.Services.Common
    public class CurrentUserService : ICurrentUserService
    {
        private readonly AuthenticationStateProvider _authProvider;

        public CurrentUserService(AuthenticationStateProvider authProvider)
        {
            _authProvider = authProvider;
        }

        private AuthenticationState GetState() =>
            _authProvider.GetAuthenticationStateAsync().GetAwaiter().GetResult();

        public string? UserId =>
            GetState().User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        public string? UserName =>
            GetState().User.Identity?.Name;

        public IEnumerable<long> BranchIds =>
            GetState().User.FindAll("branch_id")
                .Select(c => long.TryParse(c.Value, out var id) ? id : 0)
                .Where(id => id > 0);

        public bool IsInRole(string role) =>
            GetState().User.IsInRole(role);
    }
}
