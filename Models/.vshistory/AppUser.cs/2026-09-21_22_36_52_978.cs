using Microsoft.AspNetCore.Identity;

namespace Lumen.Models
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        // Which branch(es) does this user belong to?
        public ICollection<UserBranch> Branches { get; set; } = new List<UserBranch>();
    }

    public class CurrentUserState
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
    }




    public class UserListItem
    {
        public string Id { get; set; } = "";
        public string UserName { get; set; } = "";
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public List<string> Roles { get; set; } = new();
        public List<long> BranchIds { get; set; } = new();
        public List<string> BranchNames { get; set; } = new();
    }

    public class CreateUserDto
    {
        public string UserName { get; set; } = "";
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string Password { get; set; } = "";
        public string ConfirmPassword { get; set; } = "";
        public List<string> Roles { get; set; } = new();
        public List<long> BranchIds { get; set; } = new();
        public bool IsActive { get; set; } = true;
    }

    public class UpdateUserDto
    {
        public string UserId { get; set; } = "";
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public List<string> Roles { get; set; } = new();
        public List<long> BranchIds { get; set; } = new();
        public bool IsActive { get; set; } = true;
    }

    public class ChangePasswordDto
    {
        public string UserId { get; set; } = "";
        public string NewPassword { get; set; } = "";
        public string ConfirmPassword { get; set; } = "";
    }
}
