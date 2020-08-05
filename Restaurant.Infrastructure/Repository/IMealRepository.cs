﻿using Restaurant.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.Repository
{
    public interface IMealRepository : IRepository<Meal>
    {
        Task<IEnumerable<Meal>> GetMeals();
    }
}
