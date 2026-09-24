using Microsoft.EntityFrameworkCore;
using ThePantry.Api.Common.Database;

namespace ThePantry.Api.Features.Categories;

public static class DeleteCategory
{
    public static void MapDeleteCategoryEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/categories/{id:guid}", async (
            Guid id,
            AppDbContext dbContext) =>
        {
            var category = await dbContext.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category is null)
            {
                return Results.NotFound(new { Error = "Kategorin hittades inte." });
            }

            var hasProducts = await dbContext.Products
                .AnyAsync(p => p.CategoryId == id);

            if (hasProducts)
            {
                return Results.BadRequest(new { Error = "Kategorin innehåller produkter och kan inte tas bort. Flytta eller ta bort produkterna först." });
            }

            dbContext.Categories.Remove(category);
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        }).RequireAuthorization();
    }
}