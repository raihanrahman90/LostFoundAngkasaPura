using System.ComponentModel.DataAnnotations.Schema;

namespace LostFoundAngkasaPura.DAL.Model
{
    public class AdminNotification : BaseModel
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Url { get; set; }
        public string ItemClaimId { get; set; }
        [ForeignKey("ItemClaimId")]
        public ItemClaim ItemClaim { get; set; }
        public string AdminId { get; set; }
        [ForeignKey("AdminId")]
        public Admin Admin { get; set; }
    }
}
