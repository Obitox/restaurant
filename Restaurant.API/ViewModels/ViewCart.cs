﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.API.ViewModels
{
    public class ViewCart
    {
        public List<ViewCartItem> Items { get; set; }
        public List<ViewCartMeal> Meals { get; set; }
    }
}
