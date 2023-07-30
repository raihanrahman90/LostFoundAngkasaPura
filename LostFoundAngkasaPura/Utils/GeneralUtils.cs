using LostFoundAngkasaPura.DTO.Error;
using Org.BouncyCastle.Asn1.Ocsp;
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

        public static string UploadFile(string base64, string location)
        {
            if (File.Exists(location)) File.Delete(location);
            byte[] imageByte = Convert.FromBase64String(base64);
            File.WriteAllBytes(location, imageByte);
            return location;
        }

        public static (string, string) GetDetailImageBase64(string base64)
        {
            try
            {
                var extension = base64.Split(';')[0].Split('/')[1];
                var image = base64.Split(',')[1];
                if (!Constant.Constant.ValidImageExtension.Contains(extension.ToLower()))
                    throw new DataMessageError(ErrorMessageConstant.ImageNotValid);
                return (extension, image);
            }catch(Exception e)
            {
                throw new DataMessageError(ErrorMessageConstant.ImageNotValid);
            }
        }


    }
}
