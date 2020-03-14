using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Restaurant.Infrastructure.Context;
using Restaurant.Infrastructure.Models;
using Restaurant.Infrastructure.Repository;


namespace RestaurantAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly IItemRepository _repository;
        private readonly ILogger _logger;

        public HomeController(IItemRepository repository, ILogger<HomeController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Items()
        {
            _logger.LogInformation("Entered Data");
            var items = await _repository.GetItems();
            _logger.LogInformation("Exiting Data");
            return Ok(items);
        }
    }
}
