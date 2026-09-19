using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Lumen.Services.UserServices
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly AuthenticationStateProvider _auth;
        private AuthenticationState? _state;

        public CurrentUserService(AuthenticationStateProvider auth) => _auth = auth;

        public string? UserId => (_state ??= _auth.GetAuthenticationStateAsync().Result)
            .User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public string? UserName => (_state ??= _auth.GetAuthenticationStateAsync().Result)
            .User.Identity?.Name;

        public IEnumerable<long> BranchIds => (_state ??= _auth.GetAuthenticationStateAsync().Result)
            .User.FindAll("branch_id").Select(c => long.Parse(c.Value));
    }
}
