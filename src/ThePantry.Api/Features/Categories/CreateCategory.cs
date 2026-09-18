using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ThePantry.Api.Common.Database;
using ThePantry.Api.Common.Entities;

namespace ThePantry.Api.Features.Categories;

public static class CreateCategory
{
    public record Request(string Name);
    public record Response(Guid Id, string Name);

    public static void MapCreateCategoryEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/categories", async (
            Request request,
            ClaimsPrincipal user,
            AppDbContext dbContext) =>
        {
            var householdIdClaim = user.FindFirst("household_id")?.Value;

            if (!Guid.TryParse(householdIdClaim, out var householdId))
            {
                return Results.BadRequest(new { Error = "Du måste tillhöra ett hushåll för att skapa en kategori." });
            }

            var category = new Category
            {
                Name = request.Name,
                HouseholdId = householdId
            };

            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();

            return Results.Created(
                $"/categories/{category.Id}",
                new Response(category.Id, category.Name));
        }).RequireAuthorization();
    }
}