namespace LostFoundAngkasaPura.Service.Mailer
{
    public interface IMailerService
    {
        public Task CreateAdmin(string email, string name, string password);
    }
}
