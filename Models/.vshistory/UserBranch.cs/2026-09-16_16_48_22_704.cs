namespace Lumen.Models
{
    public class UserBranch : BaseEntity
    {
        public long UserBranchId { get; set; }
        public string UserId { get; set; }      // matches AspNetUsers.Id
        public long BranchId { get; set; }

        public bool IsPrimary { get; set; }     // default branch when logging in
        public DateTime AssignedAt { get; set; }

        public AppUser User { get; set; }
        public Branch Branch { get; set; }
    }
}
