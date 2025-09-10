namespace CurrencyConversion.API.Models
{
    public class ConversionRequest
    {
        public string? Currency { get; set; }
        public decimal Amount { get; set; }
    }
}
