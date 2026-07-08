namespace NextcloudUrlUploader;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using BionicCode.Utilities.Net;

internal sealed class UploadCommand
{
    public UploadCommand(
        IEnumerable<Uri> sourceUrls,
        int sourceUrlCount,
        Uri shareLink,
        DirectoryDescriptor subfolder,
        bool isOverwriteEnabled,
        string password)
    {
        ArgumentNullException.ThrowIfNull(shareLink);
        ArgumentNullException.ThrowIfNull(password);
        _remoteShare = RemoteShareBuilder.Create(shareLink, new Credentials(password, string.Empty), subfolder);
        FileUploader = new FileUploader();
        FileDownloader = new FileDownloader();
        SourceUrls = sourceUrls;
        SourceUrlCount = sourceUrlCount;
        IsOverwriteEnabled = isOverwriteEnabled;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (UploadInfo == default)
        {
            UploadInfo = await UploadInfoBuilder.CreateAsync(_remoteShare, Mode.FileDrop, IsOverwriteEnabled, cancellationToken).ConfigureAwait(false);
        }

        int fileCount = 0;
        int failedCount = 0;
        foreach (Uri sourceUrl in SourceUrls)
        {
            cancellationToken.ThrowIfCancellationRequested();

            fileCount++;
            Stream fileStream = await FileDownloader.DownloadFileAsync(sourceUrl, s_httpClient, cancellationToken).ConfigureAwait(false);
            if (fileStream.Length == 0)
            {
                throw new InvalidOperationException("The downloaded file is empty.");
            }

            cancellationToken.ThrowIfCancellationRequested();

            HttpResponseMessage response = await FileUploader.UploadAsync(fileStream, UploadInfo, s_httpClient, cancellationToken).ConfigureAwait(false);

            failedCount = ReportProgress(fileCount, failedCount, sourceUrl, response);
        }

        Console.WriteLine();
        string uploadSummary = $"Upload completed. {fileCount - failedCount} of {fileCount} files uploaded successfully, {failedCount} files failed.";
        uploadSummary.WriteLineToConsole(ConsoleColor.Blue);
    }

    private int ReportProgress(int fileCount, int failedCount, Uri sourceUrl, HttpResponseMessage response)
    {
        ConsoleColor messageHeadForeground = ConsoleColor.Yellow;
        ConsoleColor messageBodyForeground = ConsoleColor.Green;
        if (!response.IsSuccessStatusCode)
        {
            failedCount++;
            messageBodyForeground = ConsoleColor.Red;
        }

        string messageHead = $"File {fileCount} of {SourceUrlCount}: ";
        string fileName = sourceUrl.Segments[^1];
        string messageBody = response.IsSuccessStatusCode
            ? $"{fileName}' uploaded successfully to '{UploadInfo.UploadManifest.RemoteShare.DestinationUrl}'."
            : $"{fileName}' upload failed with status code: {response.StatusCode}: {response.ReasonPhrase}";

        messageHead.WriteToConsole(messageHeadForeground);
        messageBody.WriteLineToConsole(messageBodyForeground);
        return failedCount;
    }

    public UploadInfo UploadInfo { get; private set; }
    public FileUploader FileUploader { get; }
    public FileDownloader FileDownloader { get; }
    public IEnumerable<Uri> SourceUrls { get; }
    public int SourceUrlCount { get; }
    public bool IsOverwriteEnabled { get; }

    private static readonly HttpClient s_httpClient = new();
    private readonly RemoteShare _remoteShare;
}

internal readonly record struct CommandResult(bool IsSuccessful, string Message);

internal sealed class FileUploader
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
    public async Task<HttpResponseMessage> UploadAsync(Stream fileStream, UploadInfo uploadInfo, HttpClient httpClient, CancellationToken cancellationToken)
    {
        ArgumentNullExceptionAdvanced.ThrowIfNull(fileStream);
        ArgumentNullExceptionAdvanced.ThrowIfDefault(uploadInfo);
        ArgumentNullExceptionAdvanced.ThrowIfNull(httpClient);

        var streamContent = new StreamContent(fileStream);
        var requestMessage = new HttpRequestMessage(HttpMethod.Put, uploadInfo.UploadManifest.RemoteShare.DestinationUrl)
        {
            Content = streamContent,
            Headers =
            {
                { "Authorization", $"Bearer {uploadInfo.UploadManifest.RemoteShare.Credentials.Password}" },
                { "X-Requested-With", "XMLHttpRequest" },
                { "X-NC-WebDAV-AutoMkcol", "1" },
                { "Content-Type", "application/octet-stream" },
            }
        };

        if (!uploadInfo.IsOverwriteEnabled)
        {
            requestMessage.Headers.Add("If-None-Match", "*");
        }

        cancellationToken.ThrowIfCancellationRequested();

        // TODO::Report progress of the upload operation using a progress reporting mechanism.
        // But at least provide terminal feedback.
        return await httpClient.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
    }
}

internal sealed class FileDownloader
{
    public static async Task<Stream> DownloadFileAsync(Uri sourceUrl, HttpClient httpClient, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(sourceUrl);
        ArgumentNullException.ThrowIfNull(httpClient);

        cancellationToken.ThrowIfCancellationRequested();

        Stream fileStream = await httpClient.GetStreamAsync(sourceUrl, cancellationToken).ConfigureAwait(false);
        return fileStream;
    }
}

internal static class RemoteShareBuilder
{
    public static RemoteShare Create(Uri shareLink, Credentials credentials, DirectoryDescriptor subdirectory = default)
    {
        ArgumentNullExceptionAdvanced.ThrowIfNull(shareLink);

        if (credentials == default)
        {
            credentials = Credentials.Empty;
        }

        if (subdirectory == default)
        {
            subdirectory = DirectoryDescriptor.Empty;
        }

        var baseUrl = new Uri($"{shareLink.Scheme}://{shareLink.Host}");
        ArgumentExceptionAdvanced.ThrowIfNullOrWhiteSpace(
            baseUrl.OriginalString,
            $"Invalid argument '{nameof(shareLink)}'. No base URL found.",
            nameof(shareLink));

        string shareToken = shareLink.Segments[^1];

        Uri destinationUrl = subdirectory == DirectoryDescriptor.Empty
            ? new Uri(shareLink, shareToken)
            : new Uri(shareLink, $"{shareToken}/{subdirectory}");
        return new RemoteShare(
            shareLink,
            baseUrl,
            destinationUrl,
            shareToken,
            subdirectory,
            credentials);
    }
}

internal readonly record struct UploadManifest(
    RemoteShare RemoteShare,
    Mode Mode,
    IEnumerable<FileExtension> AllowedFileExtensions);

internal readonly record struct UploadInfo(
    UploadManifest UploadManifest,
    bool IsOverwriteEnabled);

internal readonly record struct Credentials(string Password,
    string Username)
{
    public static Credentials Empty => new(string.Empty, string.Empty);
};

internal readonly record struct RemoteShare(
    Uri PublicDavRootUrl,
    Uri BaseUrl,
    Uri DestinationUrl,
    string ShareToken,
    DirectoryDescriptor RemoteSubdirectory,
    Credentials Credentials);

internal enum Mode
{
    Undefined = 0,
    FileDrop,
}

internal sealed class UploadInfoBuilder()
{
    // TODO::Check whether JSON manifest file and related entry exist and create UploadManifest instance from it.
    // If not, create a new manifest file and/or entry from the provided command arguments.
    public static async Task<UploadInfo> CreateAsync(RemoteShare remoteShare, Mode mode, bool isOverwriteEnabled, CancellationToken cancellationToken)
    {
        ArgumentNullExceptionAdvanced.ThrowIfDefault(remoteShare);
        ArgumentExceptionAdvanced.ThrowIfEnumIsNotDefined<Mode>(mode);
        ArgumentExceptionAdvanced.ThrowIfEnumEqualsAny<Mode>(mode, new Mode[] { Mode.Undefined }, nameof(mode), $"Invalid argument '{nameof(mode)}'. The value '{mode}' is not allowed.");

        cancellationToken.ThrowIfCancellationRequested();

        (bool success, UploadManifest manifest) = await GetManifestFromFileAsync(cancellationToken).ConfigureAwait(false);
        if (!success)
        {
            manifest = new UploadManifest(remoteShare, mode, Enumerable.Empty<FileExtension>());
            await SaveManifestToFileAsync(manifest, cancellationToken).ConfigureAwait(false);
        }

        return new UploadInfo(manifest, isOverwriteEnabled);
    }

    private static async Task<(bool Success, UploadManifest Manifest)> GetManifestFromFileAsync(CancellationToken cancellationToken)
    {
        if (!ApplicationInfo.ManifestFileDescriptor.TryOpenForAsyncRead(out FileStream manifestFile))
        {
            return (false, default);
        }

        cancellationToken.ThrowIfCancellationRequested();

        await using (manifestFile)
        {
            UploadManifest manifestJson = await JsonSerializer.DeserializeAsync<UploadManifest>(manifestFile, cancellationToken: cancellationToken).ConfigureAwait(false);

            return (true, manifestJson);
        }
    }

    private static async Task SaveManifestToFileAsync(UploadManifest manifest, CancellationToken cancellationToken)
    {
        ArgumentNullExceptionAdvanced.ThrowIfDefault(manifest);

        cancellationToken.ThrowIfCancellationRequested();

        await using Stream manifestFile = ApplicationInfo.ManifestFileDescriptor.OpenForAsyncWrite(isOpenOrCreateEnabled: true);
        await JsonSerializer.SerializeAsync(manifestFile, manifest, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}

internal static class ApplicationInfo
{
    private static readonly Assembly s_assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
    private const string SourcesBatchFileName = "sources.txt";
    private const string ManifestFileName = "upload_manifest.json";

    private static string? s_version;
    public static string Version => s_version ??= GetAppVersion();

    private static string? s_name;
    public static string Name => s_name ??= s_assembly.GetName().Name ?? "NextcloudUrlUploader___";
    public static FileSystemPathDescriptor SourcesBatchFileDescriptor = new(SourcesBatchFileName, new DirectoryDescriptor(Environment.CurrentDirectory));
    public static FileSystemPathDescriptor ManifestFileDescriptor = new(ApplicationInfo.ManifestFileName, new DirectoryDescriptor(Environment.CurrentDirectory));

    private static string GetAppVersion()
    {
        // Prefer AssemblyInformationalVersion for product display
        string? infoVer = s_assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        if (!string.IsNullOrEmpty(infoVer))
        {
            return infoVer;
        }

        // Fallback to FileVersion / AssemblyName version
        string? fileVer = FileVersionInfo.GetVersionInfo(s_assembly.Location).ProductVersion;
        if (!string.IsNullOrEmpty(fileVer))
        {
            return fileVer;
        }

        return s_assembly.GetName().Version?.ToString() ?? "0.0.0";
    }
}
