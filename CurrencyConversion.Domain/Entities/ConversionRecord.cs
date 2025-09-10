namespace CurrencyConversion.Domain.Entities
{
    public class ConversionRecord
    {
        public long Id { get; set; }
        public required string FromCurrency { get; set; }
        public decimal InputAmount { get; set; }
        public decimal OutputDkk { get; set; }
        public decimal RateUsed { get; set; } 
        public DateTime AsOfDate { get; set; }
        public DateTime CreatedUtc { get; set; }

    }
}
