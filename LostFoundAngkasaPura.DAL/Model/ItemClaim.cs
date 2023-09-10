using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LostFoundAngkasaPura.DAL.Model
{
    public class ItemClaim : BaseModel
    {
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public string ItemFoundId { get; set; }
        [ForeignKey("ItemFoundId")]
        public ItemFound ItemFound { get; set; }
        public string ProofImage { get; set; }
        public string ProofDescription { get; set; }
        public string IdentityType { get; set; }
        public string IdentityNumber { get; set; }
        public string Status { get; set; }
        public int? Rating { get; set; }
        [JsonIgnore]
        public List<ItemClaimApproval>? ItemClaimApproval { get; set; }
    }
}
