using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using MeetingTranscriptionBot.API.IntegrationTests.Infrastructure;

namespace MeetingTranscriptionBot.API.IntegrationTests.Meetings;

public sealed class MeetingOwnershipTests
    : IClassFixture<MeetingTranscriptionBotWebApplicationFactory>
{
    private readonly MeetingTranscriptionBotWebApplicationFactory _factory;

    public MeetingOwnershipTests(
        MeetingTranscriptionBotWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task UserB_CannotAccess_UserAsMeeting()
    {
        using var userAClient = _factory.CreateClient();
        using var userBClient = _factory.CreateClient();

        var userAEmail =
            $"user.a.{Guid.NewGuid()}@example.com";

        var userBEmail =
            $"user.b.{Guid.NewGuid()}@example.com";

        const string password = "Secure@Test2026!";

        await RegisterUserAsync(
            userAClient,
            "User",
            "A",
            userAEmail,
            password);

        await RegisterUserAsync(
            userBClient,
            "User",
            "B",
            userBEmail,
            password);

        var userAToken = await LoginAsync(
            userAClient,
            userAEmail,
            password);

        var userBToken = await LoginAsync(
            userBClient,
            userBEmail,
            password);

        userAClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                userAToken);

        var createMeetingRequest = new
        {
            title = "User A Private Meeting",
            description = "Ownership integration test",
            platform = "Zoom",
            scheduledStartTime =
                DateTime.UtcNow.AddDays(2),
            scheduledEndTime =
                DateTime.UtcNow.AddDays(2).AddHours(1)
        };

        var createResponse =
            await userAClient.PostAsJsonAsync(
                "/api/Meetings",
                createMeetingRequest);

        createResponse.StatusCode.Should().Be(
            HttpStatusCode.Created);

        var meetingId =
            await ReadMeetingIdAsync(createResponse);

        userBClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                userBToken);

        var unauthorizedAccessResponse =
            await userBClient.GetAsync(
                $"/api/Meetings/{meetingId}");

        unauthorizedAccessResponse.StatusCode.Should().Be(
            HttpStatusCode.NotFound);
    }

    private static async Task RegisterUserAsync(
        HttpClient client,
        string firstName,
        string lastName,
        string email,
        string password)
    {
        var request = new
        {
            firstName,
            lastName,
            email,
            password
        };

        var response = await client.PostAsJsonAsync(
            "/api/auth/register",
            request);

        response.StatusCode.Should().Be(
            HttpStatusCode.Created);
    }

    private static async Task<string> LoginAsync(
        HttpClient client,
        string email,
        string password)
    {
        var request = new
        {
            email,
            password
        };

        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            request);

        response.StatusCode.Should().Be(
            HttpStatusCode.OK);

        var json =
            await response.Content.ReadFromJsonAsync<JsonElement>();

        var accessToken = json
            .GetProperty("data")
            .GetProperty("accessToken")
            .GetString();

        accessToken.Should().NotBeNullOrWhiteSpace();

        return accessToken!;
    }

    private static async Task<Guid> ReadMeetingIdAsync(
        HttpResponseMessage response)
    {
        var json =
            await response.Content.ReadFromJsonAsync<JsonElement>();

        var meetingIdText = json
            .GetProperty("data")
            .GetString();

        Guid.TryParse(
                meetingIdText,
                out var meetingId)
            .Should()
            .BeTrue();

        return meetingId;
    }


    [Fact]
    public async Task UserB_List_DoesNotContain_UserAsMeeting()
    {
        using var userAClient = _factory.CreateClient();
        using var userBClient = _factory.CreateClient();

        var userAEmail = $"user.a.{Guid.NewGuid()}@example.com";
        var userBEmail = $"user.b.{Guid.NewGuid()}@example.com";

        const string password = "Secure@Test2026!";

        await RegisterUserAsync(
            userAClient,
            "User",
            "A",
            userAEmail,
            password);

        await RegisterUserAsync(
            userBClient,
            "User",
            "B",
            userBEmail,
            password);

        var userAToken = await LoginAsync(
            userAClient,
            userAEmail,
            password);

        var userBToken = await LoginAsync(
            userBClient,
            userBEmail,
            password);

        userAClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                userAToken);

        var createResponse =
            await userAClient.PostAsJsonAsync(
                "/api/Meetings",
                new
                {
                    title = "User A Hidden Meeting",
                    description = "List ownership test",
                    platform = "Microsoft Teams",
                    scheduledStartTime =
                        DateTime.UtcNow.AddDays(3),
                    scheduledEndTime =
                        DateTime.UtcNow.AddDays(3).AddHours(1)
                });

        createResponse.StatusCode.Should().Be(
            HttpStatusCode.Created);

        userBClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                userBToken);

        var listResponse = await userBClient.GetAsync(
            "/api/Meetings?PageNumber=1&PageSize=10");

        listResponse.StatusCode.Should().Be(
            HttpStatusCode.OK);

        var json =
            await listResponse.Content.ReadFromJsonAsync<JsonElement>();

        var items = json
            .GetProperty("data")
            .GetProperty("items");

        items.GetArrayLength().Should().Be(0);

        json
            .GetProperty("data")
            .GetProperty("totalCount")
            .GetInt32()
            .Should()
            .Be(0);
    }


    [Fact]
    public async Task UserB_CannotDelete_UserAsMeeting()
    {
        using var userAClient = _factory.CreateClient();
        using var userBClient = _factory.CreateClient();

        var userAEmail = $"user.a.{Guid.NewGuid()}@example.com";
        var userBEmail = $"user.b.{Guid.NewGuid()}@example.com";

        const string password = "Secure@Test2026!";

        await RegisterUserAsync(
            userAClient,
            "User",
            "A",
            userAEmail,
            password);

        await RegisterUserAsync(
            userBClient,
            "User",
            "B",
            userBEmail,
            password);

        var userAToken = await LoginAsync(
            userAClient,
            userAEmail,
            password);

        var userBToken = await LoginAsync(
            userBClient,
            userBEmail,
            password);

        userAClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                userAToken);

        var createResponse =
            await userAClient.PostAsJsonAsync(
                "/api/Meetings",
                new
                {
                    title = "User A Delete Test",
                    description = "Ownership delete test",
                    platform = "Zoom",
                    scheduledStartTime = DateTime.UtcNow.AddDays(2),
                    scheduledEndTime =
                        DateTime.UtcNow.AddDays(2).AddHours(1)
                });

        var meetingId =
            await ReadMeetingIdAsync(createResponse);

        userBClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                userBToken);

        var deleteResponse =
            await userBClient.DeleteAsync(
                $"/api/Meetings/{meetingId}");

        deleteResponse.StatusCode.Should().Be(
            HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UserB_CannotAccess_UserAsMeetingHistory()
    {
        using var userAClient = _factory.CreateClient();
        using var userBClient = _factory.CreateClient();

        var userAEmail = $"user.a.{Guid.NewGuid()}@example.com";
        var userBEmail = $"user.b.{Guid.NewGuid()}@example.com";

        const string password = "Secure@Test2026!";

        await RegisterUserAsync(
            userAClient,
            "User",
            "A",
            userAEmail,
            password);

        await RegisterUserAsync(
            userBClient,
            "User",
            "B",
            userBEmail,
            password);

        var userAToken = await LoginAsync(
            userAClient,
            userAEmail,
            password);

        var userBToken = await LoginAsync(
            userBClient,
            userBEmail,
            password);

        userAClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                userAToken);

        var createResponse =
            await userAClient.PostAsJsonAsync(
                "/api/Meetings",
                new
                {
                    title = "User A History Test",
                    description = "Ownership history test",
                    platform = "Microsoft Teams",
                    scheduledStartTime = DateTime.UtcNow.AddDays(3),
                    scheduledEndTime =
                        DateTime.UtcNow.AddDays(3).AddHours(1)
                });

        var meetingId =
            await ReadMeetingIdAsync(createResponse);

        userBClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                userBToken);

        var historyResponse =
            await userBClient.GetAsync(
                $"/api/Meetings/{meetingId}/history");

        historyResponse.StatusCode.Should().Be(
            HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task UserB_CannotUpdate_UserAsMeeting()
    {
        using var userAClient = _factory.CreateClient();
        using var userBClient = _factory.CreateClient();

        var userAEmail =
            $"user.a.{Guid.NewGuid()}@example.com";

        var userBEmail =
            $"user.b.{Guid.NewGuid()}@example.com";

        const string password = "Secure@Test2026!";

        await RegisterUserAsync(
            userAClient,
            "User",
            "A",
            userAEmail,
            password);

        await RegisterUserAsync(
            userBClient,
            "User",
            "B",
            userBEmail,
            password);

        var userAToken = await LoginAsync(
            userAClient,
            userAEmail,
            password);

        var userBToken = await LoginAsync(
            userBClient,
            userBEmail,
            password);

        userAClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                userAToken);

        var createResponse =
            await userAClient.PostAsJsonAsync(
                "/api/Meetings",
                new
                {
                    title = "User A Update Test",
                    description = "Ownership update test",
                    platform = "Zoom",
                    scheduledStartTime =
                        DateTime.UtcNow.AddDays(4),
                    scheduledEndTime =
                        DateTime.UtcNow.AddDays(4).AddHours(1)
                });

        createResponse.StatusCode.Should().Be(
            HttpStatusCode.Created);

        var meetingId =
            await ReadMeetingIdAsync(createResponse);

        userBClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                userBToken);

        var updateRequest = new
        {
            id = meetingId,
            title = "Unauthorized Update",
            description =
        "User B should not update this meeting",
            platform = "Microsoft Teams",
            scheduledStartTime =
        DateTime.UtcNow.AddDays(5),
            scheduledEndTime =
        DateTime.UtcNow.AddDays(5).AddHours(1)
        };

        var updateResponse =
            await userBClient.PutAsJsonAsync(
                $"/api/Meetings/{meetingId}",
                updateRequest);

        updateResponse.StatusCode.Should().Be(
            HttpStatusCode.NotFound);
    }

}