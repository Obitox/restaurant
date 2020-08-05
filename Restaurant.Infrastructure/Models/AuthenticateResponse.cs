using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.Models
{
    public class AuthenticateResponse
    {
        public string Username { get; set; }
        public string AuthToken { get; set; }
        public string RefreshToken { get; set; }
        public string CSRFToken { get; set; }

        public long ExpiresAt { get; set; }
    }
}
