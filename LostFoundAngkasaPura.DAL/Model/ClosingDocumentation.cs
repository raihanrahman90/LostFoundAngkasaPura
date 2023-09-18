using System.ComponentModel.DataAnnotations.Schema;

namespace LostFoundAngkasaPura.DAL.Model
{
    public class ClosingDocumentation : BaseModel
    {
        public string ItemFoundId { get; set; }
        [ForeignKey("ItemFoundId")]
        public ItemFound ItemFound { get; set; }
        public string? ItemClaimId { get; set; }
        [ForeignKey("ItemClaimId")]
        public ItemClaim ItemClaim { get; set; }
        public string TakingItemImage { get; set; }
        public string NewsDocumentation { get; set; }
        public string ClosingAgent { get; set; }

    }
}
