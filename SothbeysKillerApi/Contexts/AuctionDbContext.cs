using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SothbeysKillerApi.Controllers;
using SothbeysKillerApi.Entities;

namespace SothbeysKillerApi.Contexts;

public class AuctionDbContext : IdentityDbContext<AuctionUser, IdentityRole<Guid>, Guid>
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