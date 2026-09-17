using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace ThePantry.Api.Tests.Features.Auth;

public class RegisterTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RegisterTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsCreated()
    {
        var request = new
        {
            Email = $"test-{Guid.NewGuid()}@example.com",
            Password = "Test1234!"
        };

        var response = await _client.PostAsJsonAsync(
            "/auth/register",
            request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content
            .ReadFromJsonAsync<RegisterResponse>();

        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
        result.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task Register_WithWeakPassword_ReturnsBadRequest()
    {
        var request = new
        {
            Email = $"test-{Guid.NewGuid()}@example.com",
            Password = "123"
        };

        var response = await _client.PostAsJsonAsync(
            "/auth/register",
            request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsBadRequest()
    {
        var email = $"test-{Guid.NewGuid()}@example.com";
        var request = new
        {
            Email = email,
            Password = "Test1234!"
        };

        var firstResponse = await _client.PostAsJsonAsync(
            "/auth/register",
            request);

        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var secondResponse = await _client.PostAsJsonAsync(
            "/auth/register",
            request);

        secondResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private record RegisterResponse(Guid Id, string Email);
}
