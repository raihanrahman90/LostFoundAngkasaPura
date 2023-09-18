namespace LostFoundAngkasaPura.Service.Mailer
{
    public interface IMailerService
    {
        Task CreateAdmin(string email, string name, string password);
        Task CreateClaim(string email, string id);
        Task ApproveClaim(string email, string location, DateTime date, string url);
        Task RejectClaim(string email, string reason, string url);
        Task ForgotPassword(string email, string code);
        Task SendCommentToUser(string email, string itemClaimId);
        Task SendCommentToAdmin(string email, string itemClaimId);
    }
}
