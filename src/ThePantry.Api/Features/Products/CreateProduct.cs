using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ThePantry.Api.Common.Database;
using ThePantry.Api.Common.Entities;

namespace ThePantry.Api.Features.Products;

public static class CreateProduct
{
    public record Request(
        string Name,
        Guid CategoryId,
        Guid LocationId,
        decimal Quantity,
        Unit Unit,
        DateTime? ExpiryDate);

    public record Response(Guid Id, string Name);

    public static void MapCreateProductEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/products", async (
            Request request,
            ClaimsPrincipal user,
            AppDbContext dbContext) =>
        {
            var householdIdClaim = user.FindFirst("household_id")?.Value;

            if (!Guid.TryParse(householdIdClaim, out var householdId))
            {
                return Results.BadRequest(new { Error = "Du måste tillhöra ett hushåll för att lägga till en produkt." });
            }

            var categoryExists = await dbContext.Categories
                .AnyAsync(c => c.Id == request.CategoryId);

            if (!categoryExists)
            {
                return Results.BadRequest(new { Error = "Ogiltig kategori." });
            }

            var locationExists = await dbContext.Locations
                .AnyAsync(l => l.Id == request.LocationId);

            if (!locationExists)
            {
                return Results.BadRequest(new { Error = "Ogiltig plats." });
            }

            var product = new Product
            {
                Name = request.Name,
                CategoryId = request.CategoryId,
                LocationId = request.LocationId,
                HouseholdId = householdId,
                Quantity = request.Quantity,
                Unit = request.Unit,
                ExpiryDate = request.ExpiryDate
            };

            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();

            return Results.Created(
                $"/products/{product.Id}",
                new Response(product.Id, product.Name));
        }).RequireAuthorization();
    }
}