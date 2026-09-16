using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ThePantry.Api.Common.Database;
using ThePantry.Api.Common.Entities;

namespace ThePantry.Api.Features.Households;

public static class JoinHousehold
{
    public record Request(Guid HouseholdId);

    public static void MapJoinHouseholdEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/households/join", async (
            Request request,
            ClaimsPrincipal user,
            AppDbContext dbContext) =>
        {
            var userId = Guid.Parse(user.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);

            var alreadyInAHousehold = await dbContext.HouseholdMembers
            .AnyAsync(hm => hm.UserId == userId);

            if (alreadyInAHousehold)
            {
                return Results.BadRequest(new { Error = "Du tillhör redan ett hushåll." });
            }

            var household = await dbContext.Households
                .FirstOrDefaultAsync(h => h.Id == request.HouseholdId);

            if (household is null)
            {
                return Results.NotFound(new { Error = "Hushållet hittades inte." });
            }

            var alreadyMember = await dbContext.HouseholdMembers
                .AnyAsync(hm => hm.HouseholdId == request.HouseholdId && hm.UserId == userId);

            if (alreadyMember)
            {
                return Results.BadRequest(new { Error = "Du är redan medlem i detta hushåll." });
            }

            var member = new HouseholdMember
            {
                UserId = userId,
                HouseholdId = household.Id,
                Role = HouseholdRole.Member
            };

            dbContext.HouseholdMembers.Add(member);
            await dbContext.SaveChangesAsync();

            return Results.Ok(new { Message = "Du har gått med i hushållet." });
        }).RequireAuthorization();
    }
}