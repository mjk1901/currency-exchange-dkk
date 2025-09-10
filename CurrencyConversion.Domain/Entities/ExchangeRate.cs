namespace CurrencyConversion.Domain.Entities
{
    public class ExchangeRate
    {
        public int Id { get; set; }
        public required string CurrencyCode { get; set; } 
        public decimal DkkPerUnit { get; set; }          
        public DateTime AsOfDate { get; set; }           
        public DateTime UpdatedUtc { get; set; }         

    }
}
