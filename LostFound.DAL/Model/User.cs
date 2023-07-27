using System.ComponentModel.DataAnnotations.Schema;

namespace LostFoundAngkasaPura.DAL.Model
{
    public class User : BaseModel
    {
        public string? Password { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string RefreshToken { get; set; }
    }
}
