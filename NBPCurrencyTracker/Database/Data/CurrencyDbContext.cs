using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CurrencyTracker.Database.Models;

namespace CurrencyTracker.Database.Data;

public partial class CurrencyDbContext : DbContext
{
    public CurrencyDbContext()
    {
    }

    public CurrencyDbContext(DbContextOptions<CurrencyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Currency> Currencies { get; set; }

    public virtual DbSet<CurrencyExchangeRate> CurrencyExchangeRates { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-KPOFHKI6\\SQLEXPRESS;Database=APIDatabase;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Currency__3214EC074D351A01");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<CurrencyExchangeRate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Currency__3214EC07996D4D22");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Currency).WithMany(p => p.CurrencyExchangeRates)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Child_Parent");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
