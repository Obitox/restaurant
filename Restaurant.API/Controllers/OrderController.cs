using Microsoft.AspNetCore.Mvc;
using Restaurant.Domain.Services;
using Restaurant.API.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurant.DAL.MySQL.Models;

namespace Restaurant.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Index(ViewOrder order)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var items = new List<Item>();
            if (order.Cart.Items.Count > 0) {
                foreach (var cartItem in order.Cart.Items)
                {
                    var item = new Item
                    {
                        ItemId = cartItem.Id,
                        Title = cartItem.Title,
                        CartItem = new List<CartItem>
                        {
                            new CartItem
                            {
                                ItemId = cartItem.Id,
                                Amount = cartItem.Amount,
                                PersonalPreference = cartItem.PersonalPreference,
                                PortionId = cartItem.Portion.Id
                            }
                        }
                    };
                    items.Add(item);
                }
            }

            var meals = new List<Meal>();
            if(order.Cart.Meals.Count > 0)
            {
                foreach (var cartMeal in order.Cart.Meals)
                {
                    var meal = new Meal
                    {
                        MealId = cartMeal.MealId,
                        Title = cartMeal.Title,
                        CartMeal = new List<CartMeal> {
                            new CartMeal
                            {
                                MealId = cartMeal.MealId,
                                Amount = cartMeal.Amount,
                                PersonalPreference = cartMeal.PersonalPreference
                            }
                        }
                    };
                    meals.Add(meal);
                }
            }

            var orderNew = await _orderService.Process(14, 1, items, meals, order.Price);

            return Ok(orderNew);
        }
    }
}
