using LostFoundAngkasaPura.DTO.Error;
using System.Text;

namespace LostFoundAngkasaPura.Utils
{
    public class GeneralUtils
    {
        public static void IsSameUser(string userId, string tokenUserId)
        {
            if (!userId.Equals(tokenUserId)) throw new NotAuthorizeError();
        }

        public static string GetRandomPassword(int length)
        {
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();

            for (int i = 0; i < length; i++)
            {
                int index = rnd.Next(chars.Length);
                sb.Append(chars[index]);
            }

            return sb.ToString();
        }
    }
}
