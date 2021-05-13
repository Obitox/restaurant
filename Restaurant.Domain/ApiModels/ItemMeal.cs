namespace Restaurant.Domain.ApiModels
{
    public class ItemMeal
    {
        public ulong ItemId { get; set; }
        public virtual Item Item { get; set; }
    }
}