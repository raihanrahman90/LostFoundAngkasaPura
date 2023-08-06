namespace LostFoundAngkasaPura.DTO.Dashboard
{
    public class DashboardGrafikData
    {
        public List<string> Labels { get; set; }
        public List<DashboardDataset> Datasets { get; set; }
    }

    public class DashboardDataset
    {
        public string Label { get; set; }
        public List<int> Data { get; set; }
        public string BorderColor { get; set; }
        public string BackgroundColor { get; set; }
    }
}
