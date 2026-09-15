using Microsoft.AspNetCore.Identity;
using ThePantry.Api.Common.Entities;

namespace ThePantry.Api.Features.Auth;

public static class Register
{
    public record Request(string Email, string Password);

    public static void MapRegisterEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/register", async (
            Request request,
            UserManager<User> userManager) =>
        {
            var user = new User
            {
                UserName = request.Email,
                Email = request.Email
            };

            var result = await userManager.CreateAsync(user, request.Password);

            if(!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return Results.BadRequest(new { Errors = errors });
            }

            return Results.Created($"/users/{user.Id}", new { user.Id, user.Email });
        });
    }
}