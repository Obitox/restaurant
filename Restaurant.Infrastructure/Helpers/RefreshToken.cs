using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Infrastructure.Helpers
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public long Expiration { get; set; }
    }
}
