using static CurrencyConversion.Utility.Enums;

namespace CurrencyConversion.API.Models
{
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = UserRole.User.ToString();
    }
}
