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
            public readonly static string Found = "Found";
            public readonly static string Confirmation = "Confirmation";
            public readonly static string Confirmed = "Confirmed";
            public readonly static string Claimed = "Claimed";
            public readonly static string Closed = "Closed";
            public readonly static string Approved = "Approved";
            public readonly static string Rejected = "Rejected";
        }

        public class GrafikType
        {
            public readonly static string Average = "Average";
            public readonly static string Group = "Group";
        }
    }
}
