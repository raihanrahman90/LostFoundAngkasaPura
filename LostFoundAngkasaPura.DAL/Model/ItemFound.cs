using System.ComponentModel.DataAnnotations.Schema;

namespace LostFoundAngkasaPura.DAL.Model
{
    public class ItemFound : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Image { get; set; }
        public string Status { get; set; }
        public string AdminId { get; set; }
        public string ClosingImage { get; set; }
        public DateTime FoundDate { get; set; }
        [ForeignKey("AdminId")]
        public Admin Admin { get; set; }
    }
}
