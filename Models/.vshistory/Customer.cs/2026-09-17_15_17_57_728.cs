using Lumen.Services.CommonServices;
using System.ComponentModel.DataAnnotations;

namespace Lumen.Models
{
    public class Customer : BaseEntity, IHasCreatedAt, IHasCreatedBy
    {
        public long CustomerId { get; set; }

        [MaxLength(200)]
        public string FullName { get; set; } = "";

        [MaxLength(20)]
        public string PhoneNumber { get; set; } = "";   // unique across the system

        [MaxLength(1000)]
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedByUserId { get; set; }

        // Optional: does the customer always use eco or business pricing?
        public bool IsBusiness { get; set; }

        // Which branch did they first register in?
        public long? RegisteredBranchId { get; set; }
        public Branch? RegisteredBranch { get; set; }
    }
}
