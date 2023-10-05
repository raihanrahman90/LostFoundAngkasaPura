namespace LostFoundAngkasaPura.Utils
{
    public class UploadLocation
    {
        private string _uploadLocation;
        private string _urlStatic;
        private string _urlWebsite;

        public UploadLocation(IConfiguration configuration)
        {
            _uploadLocation = configuration.GetValue<string>("Base:Location");
            _urlStatic = configuration.GetValue<string>("Base:Url");
            _urlWebsite = configuration.GetValue<string>("Base:Website");
        }

        public string Url(string path)
        {
            return $"{_urlStatic}/{path}";
        }

        public string FolderLocation(string fileName)
        {
            return $"{_uploadLocation}/{fileName}";
        }

        public string ItemFoundLocation(string fileName)
        {
            return $"item-found/{fileName}";
        }

        public string ItemClaimLocation(string fileName)
        {
            return $"claim/{fileName}";
        }

        public string ComentarLocation(string fileName)
        {
            return $"comentar/{fileName}";
        }

        public string ClosingLocation(string fileName)
        {
            return $"closing/{fileName}";
        }

        public string WebsiteUrl(string path)
        {
            return $"{_urlWebsite}/{path}";
        }

        public string UserClaimPath(string itemClaimId)
        {
            return WebsiteUrl($"Claim/{itemClaimId}");
        }

        public string AdminClaimPath(string itemClaimId)
        {
            return WebsiteUrl($"Admin/ItemClaim/{itemClaimId}");
        }
    }
}
