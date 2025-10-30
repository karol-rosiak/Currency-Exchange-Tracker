using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTracker.Database.Models;

[Table("CurrencyRate")]
public partial class CurrencyRate
{
    [Key]
    public int Id { get; set; }

    public Guid CurrencyId { get; set; }

    [Column(TypeName = "decimal(7, 4)")]
    public decimal Ask { get; set; }

    [Column(TypeName = "decimal(7, 4)")]
    public decimal Bid { get; set; }

    public DateOnly ExchangeDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    public Guid? CreatedBy { get; set; }

    [ForeignKey("CurrencyId")]
    [InverseProperty("CurrencyRates")]
    public virtual Currency Currency { get; set; } = null!;
}
