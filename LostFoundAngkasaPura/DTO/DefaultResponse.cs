namespace LostFoundAngkasaPura.DTO
{
    public class DefaultResponse<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public T Data { get; set; }
        public DefaultResponse(T data)
        {
            Success = true;
            StatusCode = 200;
            Data = data;
        }
        public DefaultResponse() { }
    }
}
