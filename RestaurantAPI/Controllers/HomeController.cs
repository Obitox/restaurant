using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Restaurant.Infrastructure.Helpers;
using Restaurant.Infrastructure.Models;
using Restaurant.Infrastructure.Repository;
using Restaurant.Infrastructure.Services;
using Restaurant.Infrastructure.ViewModels;

namespace RestaurantAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        private readonly IItemRepository _itemRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMealRepository _mealRepository;
        private readonly IPortionRepository _portionRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IUserService _userService;
        private readonly ILogger _logger;

        public HomeController(IItemRepository itemRepository, ICategoryRepository categoryRepository, IMealRepository mealRepository, IPortionRepository portionRepository, IIngredientRepository ingredientRepository, IUserService userService, ILogger<HomeController> logger)
        {
            _itemRepository = itemRepository;
            _categoryRepository = categoryRepository;
            _mealRepository = mealRepository;
            _portionRepository = portionRepository;
            _ingredientRepository = ingredientRepository;
            _userService = userService;
            _logger = logger;
        }

        //[HttpPost]
        //[EnableCors]
        //public async Task<IActionResult> Index()
        //{
        //    _logger.LogInformation("Entered Data");
        //    IEnumerable<Item> items = await _itemRepository.GetItems();
        //    _logger.LogInformation("Exiting Data");
        //    return Ok(items);
        //}

        [HttpPost]
        [EnableCors]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Entered Data");
            IEnumerable<Item> items = await _itemRepository.GetItems();
            IEnumerable<Portion> portions = await _portionRepository.GetPortions();

            List<ViewItem> viewItems = new List<ViewItem>();
            List<ViewPortion> viewPortions = new List<ViewPortion>();
            ViewPortion viewPortion = new ViewPortion();

            foreach (Item item in items)
            {
                ViewItem viewItem = new ViewItem
                {
                    Id = item.ItemId,
                    CategoryId = item.CategoryId,
                    Title = item.Title
                };

                foreach (Portion portion in portions)
                {
                    foreach (CategoryPortion categoryPortion in portion.CategoryPortion)
                    {
                        if (viewPortion.Id == 0)
                            if (item.CategoryId == categoryPortion.CategoryId)
                            {
                                viewPortion.Id = categoryPortion.PortionId;
                                viewPortion.Title = portion.Title;
                                viewPortion.Price = item.Price * portion.PriceMultiplier;
                                viewPortion.CalorieCount = item.CalorieCount * portion.MassCalorieMultiplier;
                                viewPortion.Mass = item.Mass * portion.MassCalorieMultiplier;

                                viewPortions.Add(viewPortion);
                            }
                    }
                    viewPortion = new ViewPortion();
                }

                viewItem.Portions = viewPortions;
                viewItems.Add(viewItem);
                viewPortions = new List<ViewPortion>();
            }

            _logger.LogInformation("Exiting Data");
            return Ok(viewItems);
        }


        [HttpPost("categories")]
        [EnableCors]
        public IActionResult Categories()
        {
            IEnumerable<Category> categories = _categoryRepository.GetCategories();
            return Ok(categories);
        }

        [HttpPost("meals")]
        [EnableCors]
        public async Task<IActionResult> Meals()
        {
            IEnumerable<Meal> meals = await _mealRepository.GetMeals();
            return Ok(meals);
        }

        [HttpPost("portions")]
        [EnableCors]
        public async Task<IActionResult> Portions()
        {
            IEnumerable<Portion> portions = await _portionRepository.GetPortions();
            return Ok(portions);
        }

        [HttpPost("ingredients")]
        [EnableCors]
        public async Task<IActionResult> Ingredients()
        {
            //List<Ingredient> ingredients = await _ingredientRepository.GetAllIngredientsByItemId(id);
            List<Ingredient> ingredients = await _ingredientRepository.GetAllIngredients();
            return Ok(ingredients);
        }

        [HttpPost("register")]
        [EnableCors]
        public async Task<IActionResult> Register(User user)
        {
            if (!ModelState.IsValid)
                return BadRequest("User not created");

            string token = Crypto.GenerateEmailConfirmationToken();

            user.Role = "customer";

            User createdUser = await _userService.Register(user);
            bool isSent = false;

            if (createdUser != null)
            {
                string confirmLink = Url.Action("ConfirmEmail", "Account", new { userId = createdUser.UserId.ToString(), token }, Request.Scheme);
                isSent = _userService.SendConfirmatiomEmail($"{createdUser.FirstName} {createdUser.LastName}", createdUser.Email, confirmLink, token, createdUser.UserId.ToString());
            }

            if (isSent)
                return Ok("Successful registration, please confirm your email in order to login.");

            return BadRequest("Registration failed");
        }

        [HttpPost("login")]
        [EnableCors]
        public IActionResult Login(AuthenticateModel authenticateModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data submitted");
            }

            AuthenticateResponse response = _userService.Authenticate(authenticateModel.Username, authenticateModel.Password, HttpContext);

            if (response != null)
            {
                return Ok(response);
            }

            return Unauthorized("Wrong username or password.");
        }

        [HttpPost("token/refresh")]
        public IActionResult RefreshToken(RefreshModel refreshModel)
        {
            if (!ModelState.IsValid)
                return Unauthorized();

            AuthenticateResponse response = _userService.RefreshToken(refreshModel.Username, refreshModel.RefreshToken);

            if (response == null)
                return Unauthorized();

            return Ok(response);
        }

        [HttpPost("token/revoke")]
        public IActionResult RevokeRefreshToken(RefreshModel refreshModel)
        {
            if (!ModelState.IsValid)
                return Unauthorized();

            bool isRevoked = _userService.RevokeRefreshToken(refreshModel.Username, refreshModel.RefreshToken);

            if (!isRevoked)
                return Unauthorized();

            return Ok("Revoked");
        }
    }
}
