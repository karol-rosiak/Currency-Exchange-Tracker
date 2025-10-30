using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTracker.Data.Entities;

[Table("CurrencyExchangeRate")]
[Index("BaseCurrencyId", "TargetCurrencyId", "ExchangeDate", Name = "UQ_ExchangeDaily", IsUnique = true)]
public partial class CurrencyExchangeRate
{
    [Key]
    public Guid Id { get; set; }

    public Guid BaseCurrencyId { get; set; }

    public Guid TargetCurrencyId { get; set; }

    [Column(TypeName = "decimal(7, 4)")]
    public decimal Ask { get; set; }

    [Column(TypeName = "decimal(7, 4)")]
    public decimal Bid { get; set; }

    public DateOnly ExchangeDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [ForeignKey("BaseCurrencyId")]
    [InverseProperty("CurrencyExchangeRateBaseCurrencies")]
    public virtual Currency BaseCurrency { get; set; } = null!;

    [ForeignKey("TargetCurrencyId")]
    [InverseProperty("CurrencyExchangeRateTargetCurrencies")]
    public virtual Currency TargetCurrency { get; set; } = null!;
}
