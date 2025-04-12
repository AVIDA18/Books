namespace Books_api.Models
{
    public class JWTTokenOptions
    {
        public required string ValidIssuer { get; set; }
        public required string ValidAudience { get; set; }
        public required string IssuerSigningKey { get; set; }
        public required int ExpiryDays { get; set; } = 1;
    }
}
