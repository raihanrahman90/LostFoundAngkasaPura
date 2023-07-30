namespace LostFoundAngkasaPura.Constant
{
    public class Constant
    {
        public class AdminAccess
        {
            public static string Admin = "Admin";
            public static string SuperAdmin = "SuperAdmin";
        }

        public static List<string> ValidImageExtension = new List<string>() {"jpg","jpeg","png" };

        public class ItemFoundStatus
        {
            public static string Found = "Found";
            public static string Confirmation = "Confirmation";
            public static string Confirmated = "Confirmated";
            public static string Claimed = "Claimed";
            public static string Closed = "Closed";
        }
    }
}
