namespace NextcloudUrlUploader;

using System;
using System.Collections.Generic;
using BionicCode.Utilities.Net;

internal sealed class UploadCommand
{
    public UploadCommand(
        Uri sourceUrl,
        Uri shareLink,
        DirectoryDescriptor subfolder,
        bool isOverwriteEnabled,
        string password)
    {
        ArgumentNullException.ThrowIfNull(shareLink);
        ArgumentNullException.ThrowIfNull(password);
        RemoteShare remoteShare = RemoteShareBuilder.Create(shareLink, new Credentials(password, string.Empty), subfolder);
        FileUploader = new FileUploader();
        FileDownloader = new FileDownloader();
        SourceUrl = sourceUrl;
        IsOverwriteEnabled = isOverwriteEnabled;
    }

    public async Task ExecuteAsync()
    {
        // TODO::Create UploadInfo from _remoteShare and other parameters (e.g., source URL, overwrite flag).
        Stream fileStream = await FileDownloader.DownloadFileAsync(UploadInfo.SourceUrl, s_httpClient).ConfigureAwait(false);
        if (fileStream.Length == 0)
        {
            throw new InvalidOperationException("The downloaded file is empty.");
        }
        FileUploader.UploadAsync
    }

    public UploadInfo UploadInfo { get; }
    public FileUploader FileUploader { get; }
    public FileDownloader FileDownloader { get; }
    public Uri SourceUrl { get; }
    public bool IsOverwriteEnabled { get; }

    private static readonly HttpClient s_httpClient = new();
    private readonly RemoteShare _remoteShare;
}

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

        // TODO::Report progress of the upload operation using a progress reporting mechanism.
        // But at least provide terminal feedback.
        return await httpClient.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
    }
}

internal sealed class FileDownloader
{
    public static async Task<Stream> DownloadFileAsync(Uri sourceUrl, HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(sourceUrl);
        ArgumentNullException.ThrowIfNull(httpClient);

        Stream fileStream = await httpClient.GetStreamAsync(sourceUrl).ConfigureAwait(false);
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

        var baseUrl = new Uri(shareLink.Host);
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
    Uri SourceUrl,
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

internal sealed class UploadManifestBuilder()
{
    // TODO::Check whether JSON manifest file and related entry exist and create UploadManifest instance from it.
    // If not, create a new manifest file and/or entry from the provided command arguments.
}