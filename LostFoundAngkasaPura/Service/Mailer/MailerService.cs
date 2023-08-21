using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;

namespace LostFoundAngkasaPura.Service.Mailer
{
    public class MailerService : IMailerService
    {
        private string Host;
        private int Port;
        private string Username;
        private string Password;
        private string UrlWebsite;
        public MailerService(IConfiguration configuration)
        {
            Host = configuration.GetValue<string>("SMTP:Host");
            Port = configuration.GetValue<int>("SMTP:Port");
            Username = configuration.GetValue<string>("SMTP:Username");
            Password = configuration.GetValue<string>("SMTP:Password");
            UrlWebsite = configuration.GetValue<string>("Base:Website");
        }

        public async Task ApproveClaim(string email, string location, DateTime date, string url)
        {
            var subject = "Lost & Found SAMS Sepinggan Login Credential";
            var body = "" +
                "<p>" +
                "Proses klaim barang Anda telah di-approve oleh Admin Lost & Found Bandara SAMS Sepinggan Balikpapan, " +
                $"pengambilan dapat dilakukan pada lokasi dan tempat sebagai berikut" +
                "</p>" +
                $"<p>Lokasi : {location}</p>" +
                $"<p>Tanggal: {date.ToString("yyyy-MM-dd")}</p>" +
                $"<p>Untuk melihat detail pengambilan, silahkan masuk ke <a href='{url}'>link</a>";
            var html = TemplateEmail(body);
            await Send(email, subject, html);
        }

        public async Task CreateAdmin(string email, string name, string password)
        {
            var subject = "Lost & Found SAMS Sepinggan Login Credential";
            var body = "" +
                "<p>" +
                "Email Anda telah terdaftar sebagai Admin pada akun Lost & Found Bandara SAMS Sepinggan Balikpapan, " +
                $"silahkan login pada <a href='{UrlWebsite}/admin'>link</a> dengan menggunakan authentication berikut" +
                "</p>" +
                $"<p>Email: {email}</p>" +
                $"<p>Password: {password}</p>";
            var html = TemplateEmail(body);
            await Send(email, subject, html);
        }

        public async Task CreateClaim(string email, string id)
        {
            var subject = "Lost & Found SAMS Sepinggan User Claim Item";
            var body = "" +
                "<p>" +
                "User melakukan claim terhadap item yang Anda upload, lihat detail claim pada lin berikut" +
                "</p>" +
                $"<a href='{UrlWebsite}/admin/ItemClaim/{id}'>link</a></p>";
            var html = TemplateEmail(body);
            await Send(email, subject, html);
        }

        public async Task ForgotPassword(string email, string code)
        {
            var subject = "Lost & Found SAMS Sepinggan Forgot Password";
            var body = "" +
                "<p>" +
                "Masukan code berikut pada halaman lupa password untuk melakukan reset password" +
                "</p>" +
                $"<p>Code : {code}</p>";
            var html = TemplateEmail(body);
            await Send(email, subject, html);
        }

        public async Task RejectClaim(string email, string reason, string url)
        {
            var subject = "Lost & Found SAMS Sepinggan Login Credential";
            var body = "" +
                "<p>" +
                "Proses klaim barang Anda telah ditolak oleh Admin Lost & Found Bandara SAMS Sepinggan Balikpapan " +
                "dengan alasan sebagai berikut" +
                "</p>" +
                $"<p>Alasan : {reason}</p>" +
                $"<p>Untuk melihat detail pengambilan, silahkan masuk ke <a href='{url}'>link</a>";
            var html = TemplateEmail(body);
            await Send(email, subject, html);
        }

        private async Task<bool> Send(string to, string subject, string html)
        {
            try
            {
                // send email
                var _smtpClient = new SmtpClient();
                _smtpClient.Connect(Host, Port, SecureSocketOptions.StartTls);
                _smtpClient.Authenticate(Username, Password);
                // create message
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(Username));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Html) { Text = html };
                _smtpClient.Send(email);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private string TemplateEmail(string body)
        {
            return "" +
                "<html>" +
                "<head>" +
                "</head>" +
                "<body>" +
                "   <img src='https://ogfs-bpn.sepinggan-airport.com/Bandara/assets/logo.png' alt='logo'/>" +
                body +
                "</body>" +
                "</html>";
        }
    }
}
