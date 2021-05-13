using System.Collections.Generic;

namespace Restaurant.DAL.MySQL.Models
{
    public partial class Category
    {
        public Category()
        {
            CategoryPortion = new HashSet<CategoryPortion>();
            Item = new HashSet<Item>();
        }

        public ulong CategoryId { get; set; }
        public string Title { get; set; }
        public bool IsScalable { get; set; }

        public virtual ICollection<CategoryPortion> CategoryPortion { get; set; }
        public virtual ICollection<Item> Item { get; set; }
    }
}
