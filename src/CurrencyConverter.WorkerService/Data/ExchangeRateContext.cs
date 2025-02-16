using CurrencyConverter.WorkerService.Models;
using Microsoft.EntityFrameworkCore;

namespace CurrencyConverter.WorkerService.Data
{
	internal class ExchangeRateContext : DbContext
	{
		public ExchangeRateContext(DbContextOptions<ExchangeRateContext> options) : base(options)
		{
		}

		public DbSet<ExchangeRateSnapshot> Snapshots { get; set; }

		public DbSet<ExchangeRate> ExchangeRates { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<ExchangeRate>()
				.Property(e => e.Rate)
				.HasPrecision(18, 6);


			modelBuilder.Entity<ExchangeRateSnapshot>()
				.HasMany(s => s.Rates)
				.WithOne(r => r.Snapshot)
				.HasForeignKey(r => r.SnapshotId);
		}

	}
}
