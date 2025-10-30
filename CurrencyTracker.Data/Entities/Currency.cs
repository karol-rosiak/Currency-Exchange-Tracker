using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTracker.Data.Entities;

[Table("Currency")]
[Index("Code", Name = "UQ__Currency__A25C5AA7A763474D", IsUnique = true)]
public partial class Currency
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    [StringLength(3)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    [InverseProperty("BaseCurrency")]
    public virtual ICollection<CurrencyExchangeRate> CurrencyExchangeRateBaseCurrencies { get; set; } = new List<CurrencyExchangeRate>();

    [InverseProperty("TargetCurrency")]
    public virtual ICollection<CurrencyExchangeRate> CurrencyExchangeRateTargetCurrencies { get; set; } = new List<CurrencyExchangeRate>();
}
