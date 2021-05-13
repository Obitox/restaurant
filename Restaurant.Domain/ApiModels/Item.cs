﻿using System.Collections.Generic;
using Mapster;

namespace Restaurant.Domain.ApiModels
{
    public class Item
    {
        [AdaptMember("ItemId")]
        public ulong Id { get; set; }
        public string Title { get; set; }

        public ulong CategoryId { get; set; }
        public ICollection<Portion> Portions { get; set; }

        public Item(ulong id, string title, ulong categoryId, ICollection<Portion> portions)
        {
            Id = id;
            Title = title;
            Portions = portions;
            CategoryId = categoryId;
        }

        public Item()
        {
        }    
    }
}