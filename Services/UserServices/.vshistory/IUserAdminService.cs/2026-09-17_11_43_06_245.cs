using Lumen.Models;
using Microsoft.AspNetCore.Identity;

namespace Lumen.Services.UserServices
{
    public interface IUserAdminService
    {
        Task<List<UserListItem>> GetAllAsync();
        Task<List<AppRole>> GetAllRolesAsync();
        Task<List<string>> GetRolesAsync(string userId);
        Task<List<long>> GetBranchIdsAsync(string userId);
        Task<IdentityResult> CreateAsync(CreateUserDto dto);
        Task<IdentityResult> UpdateAsync(UpdateUserDto dto);
        Task<IdentityResult> ResetPasswordAsync(ChangePasswordDto dto);
        Task<IdentityResult> SetActiveAsync(string userId, bool isActive);
        Task<IdentityResult> DeleteAsync(string userId, string currentUserId);
    }
}
