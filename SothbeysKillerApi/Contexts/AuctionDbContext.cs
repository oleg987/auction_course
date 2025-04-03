using Microsoft.EntityFrameworkCore;
using SothbeysKillerApi.Controllers;

namespace SothbeysKillerApi.Contexts;

public interface IAuctionDbContext
{
    DbSet<Auction> Type { get; set; }
    int SaveChanges();
}

public class AuctionDbContext : DbContext, IAuctionDbContext
{
    public AuctionDbContext(DbContextOptions<AuctionDbContext> options) : base(options)
    {
    }
    
    public DbSet<Auction> Type { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Auction>()
            .Property(e => e.Start)
            .HasColumnType("timestamptz");
        
        modelBuilder.Entity<Auction>()
            .Property(e => e.Finish)
            .HasColumnType("timestamptz");
        
        base.OnModelCreating(modelBuilder);
    }
}