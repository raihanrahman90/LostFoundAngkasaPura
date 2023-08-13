using System.ComponentModel.DataAnnotations.Schema;

namespace LostFoundAngkasaPura.DAL.Model
{
    public class Notification : BaseModel
    {
        public string Type { get; set; }
        public string ModelId { get; set; }
        public string Url { get; set; }
    }
}
