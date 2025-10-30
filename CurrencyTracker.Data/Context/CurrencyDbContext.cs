using System;
using System.Collections.Generic;
using CurrencyTracker.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTracker.Data.Context;

public partial class CurrencyDbContext : DbContext
{
    public CurrencyDbContext(DbContextOptions<CurrencyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Currency> Currencies { get; set; }

    public virtual DbSet<CurrencyExchangeRate> CurrencyExchangeRates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Polish_100_CI_AS");

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Currency__3214EC0763DF27C2");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<CurrencyExchangeRate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Currency__3214EC07F7007936");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.BaseCurrency).WithMany(p => p.CurrencyExchangeRateBaseCurrencies)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BaseCurrency_Currency");

            entity.HasOne(d => d.TargetCurrency).WithMany(p => p.CurrencyExchangeRateTargetCurrencies)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TargetCurrency_Currency");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
