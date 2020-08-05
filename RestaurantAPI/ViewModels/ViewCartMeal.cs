using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantAPI.ViewModels
{
    public class ViewCartMeal
    {
        public ulong MealId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string PersonalPreference { get; set; }
        public uint Amount { get; set; }
    }
}
