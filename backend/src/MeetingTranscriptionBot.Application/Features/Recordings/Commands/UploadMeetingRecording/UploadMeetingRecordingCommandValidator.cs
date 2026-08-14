using FluentValidation;

namespace MeetingTranscriptionBot.Application.Features.Recordings.Commands.UploadMeetingRecording;

public sealed class UploadMeetingRecordingCommandValidator
    : AbstractValidator<UploadMeetingRecordingCommand>
{
    private static readonly string[] AllowedContentTypes =
    [
        "audio/mpeg",
        "audio/wav",
        "audio/x-wav",
        "audio/mp4",
        "audio/x-m4a"
    ];

    private const long MaxFileSizeBytes =
        100L * 1024L * 1024L;

    public UploadMeetingRecordingCommandValidator()
    {
        RuleFor(x => x.MeetingId)
            .NotEmpty();

        RuleFor(x => x.FileStream)
            .NotNull();

        RuleFor(x => x.FileName)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .Must(contentType =>
                AllowedContentTypes.Contains(
                    contentType,
                    StringComparer.OrdinalIgnoreCase))
            .WithMessage("Unsupported audio file type.");

        RuleFor(x => x.FileSizeBytes)
            .GreaterThan(0)
            .LessThanOrEqualTo(MaxFileSizeBytes)
            .WithMessage(
                "Recording file cannot exceed 100 MB.");
    }
}