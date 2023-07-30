using LostFoundAngkasaPura.DAL.Model;

namespace LostFoundAngkasaPura.DAL.Model
{
    public class Admin : BaseModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Unit { get; set; }
        public string Access { get; set; }
        public string RefreshToken { get; set; }
    }

}
