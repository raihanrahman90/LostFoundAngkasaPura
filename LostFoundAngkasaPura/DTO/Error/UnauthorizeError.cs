namespace LostFoundAngkasaPura.DTO.Error
{
    public class UnauthorizeError : BaseException
    {
        public UnauthorizeError() : base(System.Net.HttpStatusCode.Forbidden)
        {

        }
    }
}
