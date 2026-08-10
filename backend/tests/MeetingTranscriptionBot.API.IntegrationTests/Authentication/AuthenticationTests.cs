using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MeetingTranscriptionBot.API.IntegrationTests.Infrastructure;

namespace MeetingTranscriptionBot.API.IntegrationTests.Authentication;

public sealed class AuthenticationTests
    : IClassFixture<MeetingTranscriptionBotWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthenticationTests(
        MeetingTranscriptionBotWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        var request = new
        {
            email = "unknown.user@example.com",
            password = "WrongPassword@2026!"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            request);

        response.StatusCode.Should().Be(
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RegisterAndLogin_WithValidDetails_ReturnsAccessToken()
    {
        var registerRequest = new
        {
            firstName = "Integration",
            lastName = "User",
            email = $"integration.{Guid.NewGuid()}@example.com",
            password = "Secure@Test2026!"
        };

        var registerResponse = await _client.PostAsJsonAsync(
            "/api/auth/register",
            registerRequest);

        registerResponse.StatusCode.Should().Be(
            HttpStatusCode.Created);

        var loginRequest = new
        {
            registerRequest.email,
            registerRequest.password
        };

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/auth/login",
            loginRequest);

        loginResponse.StatusCode.Should().Be(
            HttpStatusCode.OK);

        var responseBody =
            await loginResponse.Content.ReadAsStringAsync();

        responseBody.Should().Contain(
            "\"accessToken\"");

        responseBody.Should().Contain(
            registerRequest.email);
    }
}