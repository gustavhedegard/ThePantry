using Microsoft.EntityFrameworkCore;

namespace ThePantry.Api.Common.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}