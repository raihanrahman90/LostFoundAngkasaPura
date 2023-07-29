namespace LostFoundAngkasaPura.DTO.Error
{
    public class ErrorMessageConstant
    {
        public static string EmailNotFound = "Email Not Found";
        public static string PasswordWrong = "Wrong Password";

        public static string EmailAlreadyExist = "Email Already Used";
        public static string RefreshTokenNotFound = "Refresh Token Not Found";
        public static string RefreshTokenExpired = "Refresh Token Expired";

        public static string FieldNotFound = "Field Not Found";
        public static string DataNotFound = "Data Not Found";

        public static string DataSentNotValid = "The data isn't valid, please contact the developer";

        public static string UploadFile = "Failed upload file, please contact the developer";

        public static string UserNotFound = "User not found";

        public static string NotValidField(string field) { return $"Not valid field {field}'"; }
    }
}
