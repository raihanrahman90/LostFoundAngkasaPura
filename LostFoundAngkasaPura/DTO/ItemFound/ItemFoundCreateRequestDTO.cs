namespace LostFoundAngkasaPura.DTO.ItemFound
{
    public class ItemFoundCreateRequestDTO
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string ImageBase64 { get; set; }
        public string Description { get; set; }
        public DateTime FoundDate { get; set; }

    }
}
