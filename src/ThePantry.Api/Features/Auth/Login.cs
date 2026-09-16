using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ThePantry.Api.Common.Entities;
using ThePantry.Api.Common.Database;
using Microsoft.EntityFrameworkCore;

namespace ThePantry.Api.Features.Auth;

public static class Login
{
    public record Request(string Email, string Password);
    public record Response(string Token, DateTime ExperiresAt);

    public static void MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login", async (
            Request request,
            UserManager<User> userManager,
            AppDbContext dbContext,
            IConfiguration configuration) =>
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if(user == null)
            {
                return Results.Unauthorized();
            }

            var passwordValid = await userManager.CheckPasswordAsync(user, request.Password);

            if(!passwordValid)
            {
                return Results.Unauthorized();
            }

            var householdId = await dbContext.HouseholdMembers
            .Where(hm => hm.UserId == user.Id)
            .Select(hm => (Guid?)hm.HouseholdId)
            .FirstOrDefaultAsync();

            var jwtSecret = configuration["Jwt:Secret"]!;
            var jwtIssuer = configuration["Jwt:Issuer"]!;
            var jwtAudience = configuration["Jwt:Audience"]!;
            var expiryMinutes = int.Parse(configuration["Jwt:ExpiryMinutes"]!);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if(householdId.HasValue)
            {
                claims.Add(new Claim("household_id", householdId.Value.ToString()));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Results.Ok(new Response(tokenString, expiresAt));
        });
    }
}