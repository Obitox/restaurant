﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.ViewModels
{
    public class ViewItem
    {
        public ulong Id { get; set; }
        public string Title { get; set; }

        public ulong CategoryId { get; set; }
        public ICollection<ViewPortion> Portions { get; set; }

        public ViewItem(ulong id, string title, ulong categoryId, ICollection<ViewPortion> portions)
        {
            Id = id;
            Title = title;
            Portions = portions;
            CategoryId = categoryId;
        }

        public ViewItem()
        {
        }
    }
}
