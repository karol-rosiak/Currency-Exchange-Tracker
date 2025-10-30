using System.ComponentModel.DataAnnotations;

namespace CurrencyTracker.API.Models
{
    public class CurrencyExchangeRate
    {
        public string? BaseName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Base currency code is required")]
        [StringLength(3)]
        public string BaseCode { get; set; } = string.Empty;

        public string? TargetName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Target currency code is required")]
        [StringLength(3)]
        public string TargetCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ask value is required")]
        public decimal Ask { get; set; }

        [Required(ErrorMessage = "Bid value is required")]
        public decimal Bid { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Exchange date is required")]
        public DateOnly ExchangeDate { get; set; }

    }
}
