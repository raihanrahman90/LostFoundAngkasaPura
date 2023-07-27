using System.Net;

namespace LostFoundAngkasaPura.DTO.Error
{
    public class BaseException : Exception
    {
        private readonly HttpStatusCode statusCode;
        public BaseException(HttpStatusCode statusCode, string message, Exception ex) : base(message, ex)
        {
            this.statusCode = statusCode;
        }
        public BaseException(HttpStatusCode statusCode, string message) : base(message)
        {
            this.statusCode = statusCode;
        }
        public BaseException(HttpStatusCode statusCode)
        {
            this.statusCode = statusCode;
        }
    }
}
