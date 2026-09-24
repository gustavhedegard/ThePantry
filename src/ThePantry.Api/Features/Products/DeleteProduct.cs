using Microsoft.EntityFrameworkCore;
using ThePantry.Api.Common.Database;

namespace ThePantry.Api.Features.Products;

public static class DeleteProduct
{
    public static void MapDeleteProductEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/products/{id:guid}", async (
            Guid id,
            AppDbContext dbContext) =>
        {
            var product = await dbContext.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null)
            {
                return Results.NotFound(new { Error = "Produkten hittades inte." });
            }

            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        }).RequireAuthorization();
    }
}