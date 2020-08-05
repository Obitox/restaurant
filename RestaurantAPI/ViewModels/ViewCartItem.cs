using Restaurant.Infrastructure.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantAPI.ViewModels
{
    public class ViewCartItem
    {
        public ulong Id { get; set; }
        public string Title { get; set; }
        public ViewPortion Portion { get; set; }
        public string PersonalPreference { get; set; }
        public uint Amount { get; set; }
    }
}
