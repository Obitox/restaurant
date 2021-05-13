namespace Restaurant.DAL.MySQL.Models
{
    public partial class CartMeal
    {
        public ulong CartMealId { get; set; }
        public ulong CartId { get; set; }
        public ulong MealId { get; set; }
        public uint Amount { get; set; }
        public string PersonalPreference { get; set; }

        public virtual Cart Cart { get; set; }
        public virtual Meal Meal { get; set; }
    }
}
