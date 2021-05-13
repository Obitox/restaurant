using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Restaurant.DAL.MySQL.Models;
using Restaurant.Domain.Helpers;
using Restaurant.Domain.Models;
using Restaurant.Domain.Services;

namespace Restaurant.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly IHomeService _homeService;
        private readonly ILogger _logger;

        public HomeController(IUserService userService, ILogger<HomeController> logger, IHomeService homeService)
        {
            _userService = userService;
            _logger = logger;
            _homeService = homeService;
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
            var home = await _homeService.HomeData();
            return Ok(home);
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
                var confirmLink = Url.Action("ConfirmEmail", "Account", new { userId = createdUser.UserId.ToString(), token }, Request.Scheme);
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
