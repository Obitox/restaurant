namespace Restaurant.Domain.ApiModels
{
    public class Image
    {
        public ulong ImageId { get; set; }
        public string Path { get; set; }
        public ulong? ItemId { get; set; }
        public ulong? MealId { get; set; }
    }
}