namespace Restaurant.Domain.ApiModels
{
    public class ItemIngredient
    {
        public ulong ItemIngredientId { get; set; }
        public ulong IngredientId { get; set; }
        public ulong ItemId { get; set; }
        public virtual Item Item { get; set; }
    }
}