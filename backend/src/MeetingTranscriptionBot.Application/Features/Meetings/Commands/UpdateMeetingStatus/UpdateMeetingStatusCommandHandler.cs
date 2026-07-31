using MediatR;
using MeetingTranscriptionBot.Application.Interfaces;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Commands.UpdateMeetingStatus;

public sealed class UpdateMeetingStatusCommandHandler
    : IRequestHandler<UpdateMeetingStatusCommand, bool>
{
    private readonly IMeetingRepository _repository;


    public UpdateMeetingStatusCommandHandler(
        IMeetingRepository repository)
    {
        _repository = repository;
    }


    public async Task<bool> Handle(
    UpdateMeetingStatusCommand request,
    CancellationToken cancellationToken)
    {
        var meeting = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (meeting is null)
            return false;

        foreach (var h in meeting.StatusHistory)
        {
            Console.WriteLine($"{h.Id}  {h.PreviousStatus} -> {h.NewStatus}");
        }

        meeting.UpdateStatus(request.Status);


        foreach (var h in meeting.StatusHistory)
        {
            Console.WriteLine($"{h.Id}  {h.PreviousStatus} -> {h.NewStatus}");
        }

        await _repository.UpdateAsync(meeting, cancellationToken);

        return true;
    }
}