namespace CurrencyTracker.Services.Models;

public class Currency
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;
}
