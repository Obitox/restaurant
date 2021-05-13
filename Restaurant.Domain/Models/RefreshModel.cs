using System.ComponentModel.DataAnnotations;

namespace Restaurant.Domain.Models
{
    public class RefreshModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
