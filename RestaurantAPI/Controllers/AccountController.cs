﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Infrastructure.Models;
using Restaurant.Infrastructure.Services;
using RestaurantAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            bool isConfirmed = await _accountService.ValidateEmailConfirmation(userId, token);

            if(isConfirmed)
                return Ok("You've successfully confirmed your email");

            return BadRequest("Email confirmation failed");
        }

        [HttpPost("profile")]
        //[ServiceFilter(typeof(AuthorizationFilter))]
        public IActionResult Profile()
        {
            string userId = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault().Value;

            if (string.IsNullOrEmpty(userId))
                return BadRequest("Unauthorized");

            User userProfile = _accountService.Profile(userId);

            if (userProfile != null)
                return Ok(userProfile);

            return BadRequest("Unauthorized");

        }
    }
}
