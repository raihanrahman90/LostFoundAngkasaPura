namespace LostFoundAngkasaPura.DTO.Auth
{
    public class UserUpdateRequestDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsUpdatePassword { get; set; }
        public string Password { get; set; }
    }
}
