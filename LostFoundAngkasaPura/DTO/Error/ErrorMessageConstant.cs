namespace LostFound.DTO.Error
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

        public static string OtpNotFound = "Invalid OTP";

        public static string FreeTemplateShouldNotHasPrice = "Free Template Should Not Has Price";
        public static string NotFreeTemplateShouldHasPrice = "Fill Gold and Silver Price If Template Not Free";

        public static string CannotUpdateCompanyAfterPayment = "Can't Update Company After Payment Generated";

        public static string CouponCodeExist(string code) { return $"Coupon with {code} Code already exist"; }
        public static string CouponCodeNotExist(string code) { return $"Cannot find any coupon with code {code}"; }
        public static string CouponIdNotExist = "Coupon Id Not Found";

        public static string PageAlreadyUsed = "Page already used by user, cannot update this page";

        public static string DataSentNotValid = "The data isn't valid, please contact the developer";

        public static string UploadFile = "Failed upload file, please contact the developer";

        public static string SilverMaximumPage = "This template uses Silver payment plan, you only can have 10 page for";
        public static string PageCoverExist = "This template already has cover page, please update page data before adding a new cover";

    }
}
