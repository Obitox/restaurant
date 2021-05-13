namespace Restaurant.Domain.Helpers
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public long Expiration { get; set; }
    }
}
