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
}
