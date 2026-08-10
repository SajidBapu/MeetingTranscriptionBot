using System.Net;
using FluentAssertions;
using MeetingTranscriptionBot.API.IntegrationTests.Infrastructure;

namespace MeetingTranscriptionBot.API.IntegrationTests.Meetings;

public sealed class MeetingAuthorizationTests
    : IClassFixture<MeetingTranscriptionBotWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MeetingAuthorizationTests(
        MeetingTranscriptionBotWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetMeetings_WithoutAccessToken_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync(
            "/api/Meetings?PageNumber=1&PageSize=10");

        response.StatusCode.Should().Be(
            HttpStatusCode.Unauthorized);
    }
}
