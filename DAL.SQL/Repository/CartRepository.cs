﻿using Restaurant.DAL.MySQL.Context;
using Restaurant.Infrastructure.Models;
using Restaurant.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.DAL.MySQL.Repository
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        public CartRepository(fastfood_dbContext fastfood) : base(fastfood)
        {
        }
    }
}
