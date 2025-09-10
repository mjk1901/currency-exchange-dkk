using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CurrencyConversion.Utility.Enums;

namespace CurrencyConversion.Domain.Entities
{
    public class ApplicationUser
    {
        public Guid Id { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public string Role { get; set; } = UserRole.User.ToString();
    }
}
