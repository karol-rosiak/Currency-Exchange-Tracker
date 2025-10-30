namespace CurrencyTracker.Services.Models;

public class CurrencyExchangeRate
{
    public string BaseName { get; set; } = string.Empty;

    public string BaseCode { get; set; } = string.Empty;

    public string TargetName { get; set; } = string.Empty;

    public string TargetCode { get; set; } = string.Empty;

    public decimal Ask { get; set; }

    public decimal Bid { get; set; }

    public DateOnly ExchangeDate { get; set; }

    public DateTime CreateDate { get; set; }

}
