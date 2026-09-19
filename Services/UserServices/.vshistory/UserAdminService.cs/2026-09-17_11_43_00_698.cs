using Lumen.InfraStructure;
using Lumen.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Lumen.Services.UserServices
{
    public class UserAdminService : IUserAdminService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ProjectDBContext _db;

        public UserAdminService(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            ProjectDBContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        public async Task<List<UserListItem>> GetAllAsync()
        {
            var users = await _db.Users.OrderByDescending(u => u.CreateDate).ToListAsync();
            var ids = users.Select(u => u.Id).ToList();

            var roleMap = await (
                from ur in _db.UserRoles
                join r in _db.Roles on ur.RoleId equals r.Id
                where ids.Contains(ur.UserId)
                select new { ur.UserId, RoleName = r.Name }
            ).ToListAsync();

            var branchMap = await (
                from ub in _db.UserBranches
                join b in _db.Branches on ub.BranchId equals b.BranchId
                where ids.Contains(ub.UserId)
                select new { ub.UserId, b.Title }
            ).ToListAsync();

            return users.Select(u => new UserListItem
            {
                Id = u.Id,
                UserName = u.UserName ?? "",
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                IsActive = u.IsActive,
                CreateDate = u.CreateDate,
                Roles = roleMap.Where(x => x.UserId == u.Id).Select(x => x.RoleName!).ToList(),
                BranchNames = branchMap.Where(x => x.UserId == u.Id).Select(x => x.Title).ToList()
            }).ToList();
        }

        public Task<List<AppRole>> GetAllRolesAsync() => _roleManager.Roles.ToListAsync();

        public async Task<List<string>> GetRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user == null ? new() : (await _userManager.GetRolesAsync(user)).ToList();
        }

        public Task<List<long>> GetBranchIdsAsync(string userId) =>
            _db.UserBranches.Where(x => x.UserId == userId).Select(x => x.BranchId).ToListAsync();

        public async Task<IdentityResult> CreateAsync(CreateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UserName))
                return Fail("نام کاربری الزامی است");
            if (string.IsNullOrWhiteSpace(dto.Password))
                return Fail("رمز عبور الزامی است");
            if (dto.Password != dto.ConfirmPassword)
                return Fail("رمز عبور و تکرار آن یکسان نیستند");

            if (await _userManager.FindByNameAsync(dto.UserName) != null)
                return Fail("این نام کاربری قبلاً استفاده شده است");

            var user = new AppUser
            {
                UserName = dto.UserName,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                IsActive = dto.IsActive,
                CreateDate = DateTime.UtcNow
            };

            // 👇 THIS writes the hashed password into AspNetUsers.PasswordHash
            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded) return createResult;

            if (dto.Roles.Any())
            {
                var r = await _userManager.AddToRolesAsync(user, dto.Roles.Distinct());
                if (!r.Succeeded) return r;
            }

            if (dto.BranchIds.Any())
            {
                foreach (var branchId in dto.BranchIds.Distinct())
                    _db.UserBranches.Add(new UserBranch
                    {
                        UserId = user.Id,
                        BranchId = branchId,
                        AssignedAt = DateTime.UtcNow
                    });
                await _db.SaveChangesAsync();
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null) return Fail("کاربر یافت نشد");

            user.FullName = dto.FullName;
            user.PhoneNumber = dto.PhoneNumber;
            user.IsActive = dto.IsActive;

            var up = await _userManager.UpdateAsync(user);
            if (!up.Succeeded) return up;

            var current = await _userManager.GetRolesAsync(user);
            var add = dto.Roles.Except(current).ToList();
            var rem = current.Except(dto.Roles).ToList();

            if (add.Any())
            {
                var r = await _userManager.AddToRolesAsync(user, add);
                if (!r.Succeeded) return r;
            }
            if (rem.Any())
            {
                var r = await _userManager.RemoveFromRolesAsync(user, rem);
                if (!r.Succeeded) return r;
            }

            var existing = await _db.UserBranches.Where(x => x.UserId == user.Id).ToListAsync();
            var existingIds = existing.Select(x => x.BranchId).ToList();
            var desired = dto.BranchIds.Distinct().ToList();

            _db.UserBranches.RemoveRange(existing.Where(x => !desired.Contains(x.BranchId)));
            foreach (var branchId in desired.Except(existingIds))
                _db.UserBranches.Add(new UserBranch
                {
                    UserId = user.Id,
                    BranchId = branchId,
                    AssignedAt = DateTime.UtcNow
                });

            await _db.SaveChangesAsync();
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> ResetPasswordAsync(ChangePasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NewPassword))
                return Fail("رمز عبور جدید الزامی است");
            if (dto.NewPassword != dto.ConfirmPassword)
                return Fail("رمز عبور و تکرار آن یکسان نیستند");

            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null) return Fail("کاربر یافت نشد");

            if (await _userManager.HasPasswordAsync(user))
                await _userManager.RemovePasswordAsync(user);

            // 👇 writes a NEW PasswordHash to AspNetUsers
            return await _userManager.AddPasswordAsync(user, dto.NewPassword);
        }

        public async Task<IdentityResult> SetActiveAsync(string userId, bool isActive)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Fail("کاربر یافت نشد");

            user.IsActive = isActive;
            if (!isActive)
                await _userManager.UpdateSecurityStampAsync(user);

            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> DeleteAsync(string userId, string currentUserId)
        {
            if (userId == currentUserId) return Fail("نمی‌توانید حساب خودتان را حذف کنید");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Fail("کاربر یافت نشد");

            var ub = await _db.UserBranches.Where(x => x.UserId == userId).ToListAsync();
            if (ub.Any())
            {
                _db.UserBranches.RemoveRange(ub);
                await _db.SaveChangesAsync();
            }

            return await _userManager.DeleteAsync(user);
        }

        private static IdentityResult Fail(string message) =>
            IdentityResult.Failed(new IdentityError { Description = message });
    }
}
