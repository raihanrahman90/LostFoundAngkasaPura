namespace LostFoundAngkasaPura.Service.Mailer
{
    public interface IMailerService
    {
        Task CreateAdmin(string email, string name, string password);
    }
}
