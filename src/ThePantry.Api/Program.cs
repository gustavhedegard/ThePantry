using Microsoft.EntityFrameworkCore;
using ThePantry.Api.Common.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ThePantry.Api.Common.Entities;
using ThePantry.Api.Features.Auth;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using ThePantry.Api.Common.ErrorHandling;
using ThePantry.Api.Features.Households;
using ThePantry.Api.Features.Locations;
using ThePantry.Api.Features.Categories;
using ThePantry.Api.Features.Products;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityCore<User>(options =>
    {
        options.Password.RequiredLength = 8;
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager();

var jwtSecret = builder.Configuration["Jwt:Secret"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/auth/me", (ClaimsPrincipal user) =>
{
    var userId = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
    return Results.Ok(new { UserId = userId });
}).RequireAuthorization();

app.MapRegisterEndpoint();
app.MapLoginEndpoint();
app.MapCreateHouseholdEndpoint();
app.MapJoinHouseholdEndpoint();
app.MapGetLocationsEndpoint();
app.MapCreateCategoryEndpoint();
app.MapGetCategoriesEndpoint();
app.MapCreateProductEndpoint();
app.MapGetProductsEndpoint();

app.Run();
