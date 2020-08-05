using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantAPI.ViewModels
{
    public class ViewOrder
    {
        public ViewCart Cart { get; set; }
        public decimal Price { get; set; }
    }
}
