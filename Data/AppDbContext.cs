using Microsoft.EntityFrameworkCore;
using FinancialTracker.API.Models;

namespace FinancialTracker.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<User>()
        .HasIndex(u => u.Email)
        .IsUnique();

    modelBuilder.Entity<Transaction>()
        .HasOne(t => t.User)
        .WithMany(u => u.Transactions)
        .HasForeignKey(t => t.UserId)
        .OnDelete(DeleteBehavior.NoAction);
}
}