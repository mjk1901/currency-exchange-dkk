namespace CurrencyConversion.API.Models
{
    public class ConversionResponse
    {
        public string? FromCurrency { get; set; }
        public decimal InputAmount { get; set; }
        public decimal OutputDkk { get; set; }
        public decimal RateUsed { get; set; }
        public DateTime AsOfDate { get; set; }
    }
}
