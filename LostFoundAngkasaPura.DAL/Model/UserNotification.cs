using System.ComponentModel.DataAnnotations.Schema;

namespace LostFoundAngkasaPura.DAL.Model
{
    public class UserNotification : BaseModel
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Url { get; set; }
        public string ItemClaimId { get; set; }
        [ForeignKey("ItemClaimId")]
        public ItemClaim ItemClaim { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
