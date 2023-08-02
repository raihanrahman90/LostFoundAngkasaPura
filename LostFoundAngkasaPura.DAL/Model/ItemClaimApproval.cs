using System.ComponentModel.DataAnnotations.Schema;

namespace LostFoundAngkasaPura.DAL.Model
{
    public class ItemClaimApproval : BaseModel
    {
        public string Status { get; set; }
        public string? ClaimLocation { get; set; }
        public DateTime? ClaimDate { get; set; }
        public string? RejectReason { get; set; }
    }
}
