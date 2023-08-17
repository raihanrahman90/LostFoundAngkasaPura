using System.ComponentModel.DataAnnotations.Schema;

namespace LostFoundAngkasaPura.DAL.Model
{
    public class ItemComment : BaseModel
    {
        public string ItemClaimId { get; set; }
        [ForeignKey("ItemClaimId")]
        public ItemClaim ItemClaim { get; set; }
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        public string? AdminId { get; set; }
        [ForeignKey("AdminId")]
        public Admin? Admin { get; set; }
        public string Value { get; set; }
        public string ImageLocation { get; set; }
    }
}
