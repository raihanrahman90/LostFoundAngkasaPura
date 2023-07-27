namespace LostFoundAngkasaPura.DTO.Error
{
    public class DataMessageError : BaseException
    {
        public DataMessageError(string message) : base(System.Net.HttpStatusCode.BadRequest, message) {
        }
    }
    public class NotFoundError : BaseException
    {
        public NotFoundError() : base(System.Net.HttpStatusCode.NotFound) { }
    }
    public class NotAuthorizeError : BaseException
    {
        public NotAuthorizeError() : base(System.Net.HttpStatusCode.Unauthorized) { }
    }
}
