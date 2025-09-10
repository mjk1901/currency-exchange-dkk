namespace CurrencyConversion.API.Models
{
    public class HistoryRequest
    {
        public string? Currency { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
