namespace LostFoundAngkasaPura.Service.Mailer
{
    public interface IMailerService
    {
        Task CreateAdmin(string email, string name, string password);
        Task ApproveClaim(string email, string location, DateTime date, string url);
        Task RejectClaim(string email, string reason, string url);
    }
}
