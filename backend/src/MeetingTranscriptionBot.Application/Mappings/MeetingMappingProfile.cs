using AutoMapper;
using MeetingTranscriptionBot.Application.Features.Meetings.DTOs;
using MeetingTranscriptionBot.Domain.Entities;

namespace MeetingTranscriptionBot.Application.Mappings;

public sealed class MeetingMappingProfile : Profile
{
    public MeetingMappingProfile()
    {
        CreateMap<Meeting, MeetingDto>()
            .ForMember(
                destination => destination.Status,
                options => options.MapFrom(
                    source => source.Status.ToString()));
    }
}