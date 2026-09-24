using Microsoft.EntityFrameworkCore;
using ThePantry.Api.Common.Database;

namespace ThePantry.Api.Features.Locations;

public static class DeleteLocation
{
    public static void MapDeleteLocationEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/locations/{id:guid}", async (
            Guid id,
            AppDbContext dbContext) =>
        {
            var location = await dbContext.Locations
                .FirstOrDefaultAsync(l => l.Id == id);

            if (location is null)
            {
                return Results.NotFound(new { Error = "Platsen hittades inte." });
            }

            if (location.IsProtected)
            {
                return Results.BadRequest(new { Error = "Denna plats kan inte tas bort." });
            }

            var hasProducts = await dbContext.Products
                .AnyAsync(p => p.LocationId == id);

            if (hasProducts)
            {
                return Results.BadRequest(new { Error = "Platsen innehåller produkter och kan inte tas bort. Flytta eller ta bort produkterna först." });
            }

            dbContext.Locations.Remove(location);
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        }).RequireAuthorization();
    }
}