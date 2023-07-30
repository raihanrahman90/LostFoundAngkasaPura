namespace LostFoundAngkasaPura.DTO.Error
{
    public class ErrorMessageConstant
    {
        public static string EmailNotFound = "Email tidak terhubung pada akun manapun";
        public static string PasswordWrong = "Password yang Anda masukkan salah";

        public static string EmailAlreadyExist = "Email telah digunakan oleh akun lain";
        public static string RefreshTokenNotFound = "Mohon login kembali";
        public static string RefreshTokenExpired = "Mohon login kembali";

        public static string DataNotFound = "Data tidak ditemukan";

        public static string DataSentNotValid = "Data yang Anda kirim tidak valid, mohon hubungi developer";

        public static string UploadFile = "Gagal meng-upload file, mohon hubungi developer";

        public static string UserNotFound = "User tidak ditemukan";

        public static string ImageNotValid = "File yang Anda upload tidak valid";

        public static string ImageEmpty = "Mohon upload gambar";

        public static string NotValidField(string field) { return $"Data pada '{field}' tidak valid"; }
        public static string ItemInClaimProgress = "Item tersebut sudah dalam proses claim";
    }
}
