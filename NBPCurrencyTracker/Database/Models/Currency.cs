using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTracker.Database.Models;

[Table("Currency")]
public partial class Currency
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [StringLength(3)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    public int Value { get; set; }

    [InverseProperty("Currency")]
    public virtual ICollection<CurrencyExchangeRate> CurrencyExchangeRates { get; set; } = new List<CurrencyExchangeRate>();
}
