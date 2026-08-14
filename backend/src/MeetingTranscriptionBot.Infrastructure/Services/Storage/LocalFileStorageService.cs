using MeetingTranscriptionBot.Application.Interfaces;

namespace MeetingTranscriptionBot.Infrastructure.Services.Storage;

public sealed class LocalFileStorageService : IFileStorageService
{
    private readonly string _storageRoot;

    public LocalFileStorageService()
    {
        _storageRoot = Path.Combine(
            AppContext.BaseDirectory,
            "storage",
            "recordings");

        Directory.CreateDirectory(_storageRoot);
    }

    public async Task<string> SaveAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(fileStream);

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException(
                "File name is required.",
                nameof(fileName));
        }

        if (string.IsNullOrWhiteSpace(contentType))
        {
            throw new ArgumentException(
                "Content type is required.",
                nameof(contentType));
        }

        var extension = Path.GetExtension(fileName);

        var generatedFileName =
            $"{Guid.NewGuid():N}{extension}";

        var fullPath = Path.Combine(
            _storageRoot,
            generatedFileName);

        await using var outputStream =
            new FileStream(
                fullPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 81920,
                useAsync: true);

        await fileStream.CopyToAsync(
            outputStream,
            cancellationToken);

        return fullPath;
    }

    public Task DeleteAsync(
        string storagePath,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(storagePath))
        {
            return Task.CompletedTask;
        }

        if (File.Exists(storagePath))
        {
            File.Delete(storagePath);
        }

        return Task.CompletedTask;
    }

    public Task<Stream> OpenReadAsync(
    string storagePath,
    CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(storagePath))
        {
            throw new ArgumentException(
                "Storage path is required.",
                nameof(storagePath));
        }

        if (!File.Exists(storagePath))
        {
            throw new FileNotFoundException(
                "Recording file was not found.",
                storagePath);
        }

        Stream stream =
            new FileStream(
                storagePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 81920,
                useAsync: true);

        return Task.FromResult(stream);
    }
}