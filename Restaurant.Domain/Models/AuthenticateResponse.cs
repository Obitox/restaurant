namespace Restaurant.Domain.Models
{
    public class AuthenticateResponse
    {
        public string Username { get; set; }
        public string AuthToken { get; set; }
        public string RefreshToken { get; set; }
        public string CsrfToken { get; set; }

        public long ExpiresAt { get; set; }
    }
}
