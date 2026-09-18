using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;

namespace ThePantry.Api.Tests.Features.Categories;

public class CreateCategoryTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CreateCategoryTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateCategory_WithoutToken_ReturnsUnauthorized()
    {
        var request = new { Name = "Mejeri" };

        var response = await _client.PostAsJsonAsync("/categories", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateCategory_WithoutHousehold_ReturnsBadRequest()
    {
        var token = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var request = new { Name = "Mejeri" };

        var response = await _client.PostAsJsonAsync("/categories", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCategory_WithHousehold_ReturnsCreated()
    {
        var token = await RegisterAndLoginWithHouseholdAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var request = new { Name = "Mejeri" };

        var response = await _client.PostAsJsonAsync("/categories", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<CategoryResponse>();

        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
        result.Name.Should().Be("Mejeri");
    }

    private async Task<string> RegisterAndLoginAsync()
    {
        var email = $"test-{Guid.NewGuid()}@example.com";
        var password = "Test1234!";

        await _client.PostAsJsonAsync("/auth/register", new { Email = email, Password = password });

        var loginResponse = await _client.PostAsJsonAsync("/auth/login", new { Email = email, Password = password });
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        return loginResult!.Token;
    }

    private async Task<string> RegisterAndLoginWithHouseholdAsync()
    {
        var email = $"test-{Guid.NewGuid()}@example.com";
        var password = "Test1234!";

        await _client.PostAsJsonAsync("/auth/register", new { Email = email, Password = password });

        var firstLoginResponse = await _client.PostAsJsonAsync("/auth/login", new { Email = email, Password = password });
        var firstLoginResult = await firstLoginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", firstLoginResult!.Token);

        await _client.PostAsJsonAsync("/households", new { HouseholdName = "Testhushåll" });

        var secondLoginResponse = await _client.PostAsJsonAsync("/auth/login", new { Email = email, Password = password });
        var secondLoginResult = await secondLoginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        return secondLoginResult!.Token;
    }

    private record LoginResponse(string Token, DateTime ExpiresAt);
    private record CategoryResponse(Guid Id, string Name);
}