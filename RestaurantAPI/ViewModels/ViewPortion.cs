﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.ViewModels
{
    public class ViewPortion
    {
        public ulong Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public decimal CalorieCount { get; set; }
        public decimal Mass { get; set; }

        public ViewPortion(ulong id, string title, decimal price, uint calorieCount, uint mass)
        {
            Id = id;
            Title = title;
            Price = price;
            CalorieCount = calorieCount;
            Mass = mass;
        }

        public ViewPortion()
        {
        }
    }
}
