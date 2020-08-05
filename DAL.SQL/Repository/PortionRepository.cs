using Microsoft.EntityFrameworkCore;
using Restaurant.DAL.MySQL.Context;
using Restaurant.Infrastructure.Models;
using Restaurant.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.DAL.MySQL.Repository
{
    public class PortionRepository : Repository<Portion>, IPortionRepository
    {
        public PortionRepository(fastfood_dbContext fastfood) : base(fastfood)
        {
        }

        public async Task<IEnumerable<Portion>> GetPortions()
        {
            if (_fastfood_dbContext.Database.CanConnect())
                return await _fastfood_dbContext.Portion
                                            .Include(portion => portion.CategoryPortion)
                                            .ToListAsync();

            return new List<Portion>();
        }
    }
}
