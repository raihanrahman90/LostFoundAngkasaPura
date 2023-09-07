namespace LostFoundAngkasaPura.DTO.Auth
{
    public class UserDataUpdateRequestDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool UpdatePassword { get; set; }
        public string Password { get; set; }

    }
}
