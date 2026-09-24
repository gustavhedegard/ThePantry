using Microsoft.EntityFrameworkCore;
using ThePantry.Api.Common.Database;

namespace ThePantry.Api.Features.Products;

public static class GetProducts
{
    public record Response(
        Guid Id,
        string Name,
        Guid CategoryId,
        Guid LocationId,
        decimal Quantity,
        string Unit,
        DateTime? ExpiryDate,
        bool IsExpired);

    public static void MapGetProductsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/products", async (
            AppDbContext dbContext,
            Guid? categoryId,
            Guid? locationId,
            int? expiringWithinDays,
            bool? isExpired) =>
        {
            var query = dbContext.Products.AsQueryable();

            if (categoryId is not null)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            if (locationId is not null)
            {
                query = query.Where(p => p.LocationId == locationId);
            }

            if (expiringWithinDays is not null)
            {
                var threshold = DateTime.UtcNow.AddDays(expiringWithinDays.Value);
                query = query.Where(p =>
                    p.ExpiryDate != null &&
                    p.ExpiryDate <= threshold &&
                    !p.IsExpired);
            }

            if (isExpired is not null)
            {
                query = query.Where(p => p.IsExpired == isExpired);
            }

            var products = await query
                .Select(p => new Response(
                    p.Id, p.Name, p.CategoryId, p.LocationId,
                    p.Quantity, p.Unit.ToString(), p.ExpiryDate, p.IsExpired))
                .ToListAsync();

            return Results.Ok(products);
        }).RequireAuthorization();
    }
}