using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Restaurant.Infrastructure.Models
{
    public class RefreshModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
