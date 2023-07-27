using LostFound.DAL.Model;

namespace LostFound.DAL.Model
{
    public class Admin : BaseModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Unit { get; set; }
        public string Access { get; set; } = "Admin";
        public string RefreshToken { get; set; }
    }
}
