using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ThePantry.Api.Common.Database;
using ThePantry.Api.Common.Entities;

namespace ThePantry.Api.Features.Households;

public static class CreateHousehold
{
    public record Request(string HouseholdName);
    public record Response(Guid Id, string HouseholdName);

    public static void MapCreateHouseholdEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/households", async (
            Request request,
            ClaimsPrincipal user,
            AppDbContext dbContext) =>
        {
            var userId = Guid.Parse(user.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);

            var household = new Household { HouseholdName = request.HouseholdName };
            dbContext.Households.Add(household);

            var member = new HouseholdMember
            {
                UserId = userId,
                Household = household,
                Role = HouseholdRole.Owner
            };
            dbContext.HouseholdMembers.Add(member);

            var defaultLocations = new List<Location>
            {
                new() { Name = "Kyl", Household = household, IsProtected = true },
                new() { Name = "Frys", Household = household, IsProtected = true },
                new() { Name = "Skafferi", Household = household, IsProtected = false }
            };
            dbContext.Locations.AddRange(defaultLocations);

            await dbContext.SaveChangesAsync();

            return Results.Created(
                $"/households/{household.Id}",
                new Response(household.Id, household.HouseholdName));
        }).RequireAuthorization();
    }
}