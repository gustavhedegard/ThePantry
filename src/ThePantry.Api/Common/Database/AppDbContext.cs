using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ThePantry.Api.Common.Entities;

namespace ThePantry.Api.Common.Database;

public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Household> Households => Set<Household>();
    public DbSet<HouseholdMember> HouseholdMembers => Set<HouseholdMember>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductHistory> ProductHistories => Set<ProductHistory>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var idProperty = entityType.FindProperty("Id");
            if (idProperty != null && idProperty.ClrType == typeof(Guid))
            {
                idProperty.SetDefaultValueSql("gen_random_uuid()");
            }

            var createdAtProperty = entityType.FindProperty("CreatedAt");
            if (createdAtProperty != null && createdAtProperty.ClrType == typeof(DateTime))
            {
                createdAtProperty.SetDefaultValueSql("now()");
            }
        }

        // HouseholdMember - many-to-many mellan User och Household
    builder.Entity<HouseholdMember>()
        .HasOne(hm => hm.User)
        .WithMany()
        .HasForeignKey(hm => hm.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.Entity<HouseholdMember>()
        .HasOne(hm => hm.Household)
        .WithMany()
        .HasForeignKey(hm => hm.HouseholdId)
        .OnDelete(DeleteBehavior.Cascade);

    // Location - hör till Household
    builder.Entity<Location>()
        .HasOne(l => l.Household)
        .WithMany()
        .HasForeignKey(l => l.HouseholdId)
        .OnDelete(DeleteBehavior.Cascade);

    // Category - hör till Household
    builder.Entity<Category>()
        .HasOne(c => c.Household)
        .WithMany()
        .HasForeignKey(c => c.HouseholdId)
        .OnDelete(DeleteBehavior.Cascade);

    // Product - kopplingar till Category, Location, Household
    builder.Entity<Product>()
        .HasOne(p => p.Category)
        .WithMany()
        .HasForeignKey(p => p.CategoryId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.Entity<Product>()
        .HasOne(p => p.Location)
        .WithMany()
        .HasForeignKey(p => p.LocationId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.Entity<Product>()
        .HasOne(p => p.Household)
        .WithMany()
        .HasForeignKey(p => p.HouseholdId)
        .OnDelete(DeleteBehavior.Cascade);

    // ProductHistory - Category nullable (SetNull vid borttagen kategori)
    builder.Entity<ProductHistory>()
        .HasOne(ph => ph.Category)
        .WithMany()
        .HasForeignKey(ph => ph.CategoryId)
        .OnDelete(DeleteBehavior.SetNull);

    builder.Entity<ProductHistory>()
        .HasOne(ph => ph.Household)
        .WithMany()
        .HasForeignKey(ph => ph.HouseholdId)
        .OnDelete(DeleteBehavior.Cascade);
    }
}