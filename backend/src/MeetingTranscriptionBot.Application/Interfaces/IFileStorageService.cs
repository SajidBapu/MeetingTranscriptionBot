namespace MeetingTranscriptionBot.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken);

    Task<Stream> OpenReadAsync(
        string storagePath,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        string storagePath,
        CancellationToken cancellationToken);

}