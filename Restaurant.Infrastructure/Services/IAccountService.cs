using Restaurant.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.Services
{
    public interface IAccountService
    {
        Task<bool> ValidateEmailConfirmation(string userId, string confirmToken);
        User Profile(string userId);
    }
}
