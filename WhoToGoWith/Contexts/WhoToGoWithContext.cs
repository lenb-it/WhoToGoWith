using Microsoft.EntityFrameworkCore;
using WhoToGoWith.Models.DbModels;

namespace WhoToGoWith.Contexts
{
	public class WhoToGoWithContext : DbContext
	{
		public DbSet<User> Users { get; set; }

		public DbSet<Event> Events { get; set; }

		public DbSet<ReadyForEvent> ReadyForEvents { get; set; }

		public WhoToGoWithContext(DbContextOptions<WhoToGoWithContext> options) : base(options)
		{
			Database.EnsureCreated();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<ReadyForEvent>().HasKey(k => new { k.UserName, k.EventId });
			modelBuilder.Entity<User>().HasKey(k => new { k.UserName });
		}
	}
}
