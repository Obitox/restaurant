namespace Restaurant.Domain.ApiModels
{
    public class Category
    {
        public ulong CategoryId { get; set; }
        public string Title { get; set; }
        public bool IsScalable { get; set; }    }
}