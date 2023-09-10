using System.ComponentModel.DataAnnotations.Schema;

namespace LostFoundAngkasaPura.DAL.Model
{
    public class ItemClaimApproval : BaseModel
    {
        public string ItemClaimId { get; set; }
        [ForeignKey("ItemClaimId")]
        public ItemClaim ItemClaim { get; set; }
        public string Status { get; set; }
        public string? ClaimLocation { get; set; }
        public DateTime? ClaimDate { get; set; }
        public string? RejectReason { get; set; }
        public string? AdminId { get; set; }
        [ForeignKey("AdminId")]
        public Admin? Admin { get; set; }
    }
}
