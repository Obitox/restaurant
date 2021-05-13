using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Templates
{
    public class OrderItem
    {
        public string Title { get; set; }
        public string PersonalPreference { get; set; }
        public string PortionTitle { get; set; }
        public uint Amount { get; set; }
        public decimal Price { get; set; }
    }
}
