using LostFoundAngkasaPura.DAL.Model;

namespace LostFoundAngkasaPura.DAL.Model
{
    public class Admin : BaseModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Unit { get; set; }
        public AdminAccess Access { get; set; } = AdminAccess.Admin;
        public string RefreshToken { get; set; }
    }

    public enum AdminAccess
    {
        Admin,SuperAdmin
    }
}
