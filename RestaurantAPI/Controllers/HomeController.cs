using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Infrastructure.Context;
using Restaurant.Infrastructure.Models;
using Restaurant.Infrastructure.Repository;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RestaurantAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly IItemRepository _repository;

        public HomeController(IItemRepository repository)
        {
            _repository = repository; 
        }

        // GET: /<controller>/
        [HttpGet]
        public async Task<IActionResult> Data()
        {
            var items = await _repository.GetItems();
            return Ok(items);
        }
    }
}
