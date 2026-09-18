using Microsoft.EntityFrameworkCore;
using ThePantry.Api.Common.Database;

namespace ThePantry.Api.Features.Categories;

public static class GetCategories
{
    public record Response(Guid Id, string Name);

    public static void MapGetCategoriesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/categories", async (AppDbContext dbContext) =>
        {
            var categories = await dbContext.Categories
                .Select(c => new Response(c.Id, c.Name))
                .ToListAsync();

            return Results.Ok(categories);
        }).RequireAuthorization();
    }
}