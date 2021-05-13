namespace Restaurant.DAL.MySQL.Models
{
    public partial class CategoryPortion
    {
        public ulong CategoryPortionId { get; set; }
        public ulong CategoryId { get; set; }
        public ulong PortionId { get; set; }

        public virtual Category Category { get; set; }
        public virtual Portion Portion { get; set; }
    }
}
