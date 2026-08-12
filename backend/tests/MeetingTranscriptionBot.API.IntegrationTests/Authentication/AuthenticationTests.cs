using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
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
            "\"refreshToken\"");

        responseBody.Should().Contain(
            registerRequest.email);
    }

    [Fact]
    public async Task Refresh_WithValidRefreshToken_RotatesToken_AndOldTokenFails()
    {
        var email =
            $"refresh.{Guid.NewGuid()}@example.com";

        const string password =
            "Secure@Test2026!";

        var registerRequest = new
        {
            firstName = "Refresh",
            lastName = "User",
            email,
            password
        };

        var registerResponse =
            await _client.PostAsJsonAsync(
                "/api/auth/register",
                registerRequest);

        registerResponse.StatusCode.Should().Be(
            HttpStatusCode.Created);

        var loginResponse =
            await _client.PostAsJsonAsync(
                "/api/auth/login",
                new
                {
                    email,
                    password
                });

        loginResponse.StatusCode.Should().Be(
            HttpStatusCode.OK);

        var loginJson =
            await loginResponse.Content
                .ReadFromJsonAsync<JsonElement>();

        var firstAccessToken =
            loginJson
                .GetProperty("data")
                .GetProperty("accessToken")
                .GetString();

        var firstRefreshToken =
            loginJson
                .GetProperty("data")
                .GetProperty("refreshToken")
                .GetString();

        firstAccessToken.Should()
            .NotBeNullOrWhiteSpace();

        firstRefreshToken.Should()
            .NotBeNullOrWhiteSpace();

        var firstRefreshResponse =
            await _client.PostAsJsonAsync(
                "/api/auth/refresh",
                new
                {
                    refreshToken =
                        firstRefreshToken
                });

        firstRefreshResponse.StatusCode.Should().Be(
            HttpStatusCode.OK);

        var refreshJson =
            await firstRefreshResponse.Content
                .ReadFromJsonAsync<JsonElement>();

        var secondAccessToken =
            refreshJson
                .GetProperty("data")
                .GetProperty("accessToken")
                .GetString();

        var secondRefreshToken =
            refreshJson
                .GetProperty("data")
                .GetProperty("refreshToken")
                .GetString();

        secondAccessToken.Should()
            .NotBeNullOrWhiteSpace();

        secondRefreshToken.Should()
            .NotBeNullOrWhiteSpace();

        secondRefreshToken.Should()
            .NotBe(firstRefreshToken);

        var reuseOldTokenResponse =
            await _client.PostAsJsonAsync(
                "/api/auth/refresh",
                new
                {
                    refreshToken =
                        firstRefreshToken
                });

        reuseOldTokenResponse.StatusCode.Should().Be(
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Logout_WithValidRefreshToken_RevokesToken()
    {
        var email =
            $"logout.{Guid.NewGuid()}@example.com";

        const string password =
            "Secure@Test2026!";

        var registerResponse =
            await _client.PostAsJsonAsync(
                "/api/auth/register",
                new
                {
                    firstName = "Logout",
                    lastName = "User",
                    email,
                    password
                });

        registerResponse.StatusCode.Should().Be(
            HttpStatusCode.Created);

        var loginResponse =
            await _client.PostAsJsonAsync(
                "/api/auth/login",
                new
                {
                    email,
                    password
                });

        loginResponse.StatusCode.Should().Be(
            HttpStatusCode.OK);

        var loginJson =
            await loginResponse.Content
                .ReadFromJsonAsync<JsonElement>();

        var refreshToken =
            loginJson
                .GetProperty("data")
                .GetProperty("refreshToken")
                .GetString();

        refreshToken.Should()
            .NotBeNullOrWhiteSpace();

        var logoutResponse =
            await _client.PostAsJsonAsync(
                "/api/auth/logout",
                new
                {
                    refreshToken
                });

        logoutResponse.StatusCode.Should().Be(
            HttpStatusCode.OK);

        var refreshAfterLogoutResponse =
            await _client.PostAsJsonAsync(
                "/api/auth/refresh",
                new
                {
                    refreshToken
                });

        refreshAfterLogoutResponse.StatusCode.Should().Be(
            HttpStatusCode.Unauthorized);
    }
}