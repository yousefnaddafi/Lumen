using Lumen.Services.CommonServices;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Lumen.Extensions;

namespace Lumen.Services.UserServices
{
    // Lumen.Services.Common
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthenticationStateProvider _authProvider;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor,
            AuthenticationStateProvider authProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _authProvider = authProvider;
        }

        public string? UserId
        {
            get
            {
                // 1. Try HTTP Context
                var user = _httpContextAccessor.HttpContext?.User;
                if (user?.Identity?.IsAuthenticated == true)
                    return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // 2. Try Circuit Cache
                var circuitId = _httpContextAccessor.HttpContext?.GetCircuitId();
                if (!string.IsNullOrEmpty(circuitId))
                {
                    var cachedId = CircuitUserAccessor.GetUser(circuitId);
                    if (!string.IsNullOrEmpty(cachedId)) return cachedId;
                }

                // 3. Try Auth State
                try
                {
                    var state = _authProvider.GetAuthenticationStateAsync().GetAwaiter().GetResult();
                    return state.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                }
                catch { }

                return null;
            }
        }

        // Do the same for UserName
        public string? UserName
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                if (user?.Identity?.IsAuthenticated == true)
                    return user.Identity.Name;

                try
                {
                    return _authProvider.GetAuthenticationStateAsync().GetAwaiter().GetResult().User.Identity?.Name;
                }
                catch { return null; }
            }
        }

        public IEnumerable<long> BranchIds
        {
            get
            {
                try
                {
                    var state = _authProvider.GetAuthenticationStateAsync().GetAwaiter().GetResult();
                    return state.User.FindAll("branch_id")
                        .Select(c => long.TryParse(c.Value, out var id) ? id : 0)
                        .Where(id => id > 0);
                }
                catch { return Enumerable.Empty<long>(); }
            }
        }

        public bool IsInRole(string role)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true)
                return user.IsInRole(role);

            try
            {
                return _authProvider.GetAuthenticationStateAsync().GetAwaiter().GetResult().User.IsInRole(role);
            }
            catch { return false; }
        }
    }
}
