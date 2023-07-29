namespace LostFoundAngkasaPura.DTO
{
    public class Pagination<T>
    {
        public bool IsHasMore { get; set; }
        public int Page { get; set; }
        public decimal PageTotal { get; set; }
        public int Total { get; set; }
        public List<T> Data { get; set; }
        public Pagination(List<T> data, int total, int size, int page)
        {
            Data = data;
            Total = total;
            Page = page;
            PageTotal = Math.Ceiling((decimal)total / size);
            IsHasMore = Page < PageTotal ? true : false;
        }
    }
}
