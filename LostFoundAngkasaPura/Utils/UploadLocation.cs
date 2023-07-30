namespace LostFoundAngkasaPura.Utils
{
    public class UploadLocation
    {
        private string _uploadLocation;
        private string _urlStatic;

        public UploadLocation(IConfiguration configuration)
        {
            _uploadLocation = configuration.GetValue<string>("Base:Location");
            _urlStatic = configuration.GetValue<string>("Base:Url");
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
    }
}
