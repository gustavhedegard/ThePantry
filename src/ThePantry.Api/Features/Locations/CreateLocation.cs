using System.Security.Claims;
using ThePantry.Api.Common.Database;
using ThePantry.Api.Common.Entities;

namespace ThePantry.Api.Features.Locations;

public static class CreateLocation
{
    public record Request(string Name);
    public record Response(Guid Id, string Name, bool IsProtected);

    public static void MapCreateLocationEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/locations", async (
            Request request,
            ClaimsPrincipal user,
            AppDbContext dbContext) =>
        {
            var householdIdClaim = user.FindFirst("household_id")?.Value;

            if (!Guid.TryParse(householdIdClaim, out var householdId))
            {
                return Results.BadRequest(new { Error = "Du måste tillhöra ett hushåll för att skapa en plats." });
            }

            var location = new Location
            {
                Name = request.Name,
                HouseholdId = householdId,
                IsProtected = false
            };

            dbContext.Locations.Add(location);
            await dbContext.SaveChangesAsync();

            return Results.Created(
                $"/locations/{location.Id}",
                new Response(location.Id, location.Name, location.IsProtected));
        }).RequireAuthorization();
    }
}