using Microsoft.EntityFrameworkCore;
using ThePantry.Api.Common.Database;

namespace ThePantry.Api.Features.Locations;

public static class GetLocations
{
    public record Response(Guid Id, string Name, bool IsProtected);

    public static void MapGetLocationsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/locations", async (AppDbContext dbContext) =>
        {
            var locations = await dbContext.Locations
                .Select(l => new Response(l.Id, l.Name,l.IsProtected))
                .ToListAsync();

            return Results.Ok(locations);
        }).RequireAuthorization();
    }
}