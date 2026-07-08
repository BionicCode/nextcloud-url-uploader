using System.CommandLine;
using BionicCode.Utilities.Net;
using NextcloudUrlUploader;

Console.WriteLine($"{ApplicationInfo.Name} V{ApplicationInfo.Version}");

// TODO::Support batch upload of files from a list (file) of source URLs to a Nextcloud share link.
var sourceUrlOption = new Option<string>("--sourceUrl", "--su", "--source")
{
    Description = "The URL of the file to upload."
};
var shareLinkOption = new Option<string>("--shareLink", "--sl", "--share")

{
    Description = "The share link to upload files to.",
    Required = true,
};
var subfolderOption = new Option<string>("--subdir", "--sd")
{
    Description = "The subfolder of the path speccified by the 'shareLink' argument to upload files to.",
    Required = true,
};
var isOverwriteEnabledOption = new Option<bool>("--isOverwriteEnabled", "--ow")
{
    Description = "Whether to enable overwriting of existing files.",
    DefaultValueFactory = _ => true,
    Required = false
};
var passwordOption = new Option<string>("--password", "--pw")
{
    Description = "The password for the Nextcloud account.",
    Required = true
};

// TODO::Create a initialize command to create a configuration/manifest file
// with the share link and password for future uploads and an alias that refereces a particular shar link.
var uploadCommand = new Command("upload", "Uploads files to a Nextcloud share link.")
{
    Options = { sourceUrlOption, shareLinkOption, subfolderOption, isOverwriteEnabledOption, passwordOption },
    Aliases = { "up", "u" },
    TreatUnmatchedTokensAsErrors = true
};

List<Uri> sourceUrls = new List<Uri>();
uploadCommand.SetAction(async (parserResult) =>
{
    string? sourceUrlText = parserResult.GetValue(sourceUrlOption);
    if (string.IsNullOrWhiteSpace(sourceUrlText))
    {
        if (!ApplicationInfo.SourcesBatchFileDescriptor.TryOpenForAsyncRead(out FileStream sourcesBatchFile))
        {
            throw new ArgumentException($"{sourceUrlOption.Name} is required if no sources batch file '{ApplicationInfo.SourcesBatchFileDescriptor}' is provided.", sourceUrlOption.Name);
        }

        await using (sourcesBatchFile)
        {
            using var reader = new StreamReader(sourcesBatchFile);
            string? line;
            int lineNumber = 0;
            while ((line = await reader.ReadLineAsync()) is not null)
            {
                lineNumber++;
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                if (!Uri.TryCreate(line, UriKind.Absolute, out Uri? lineSourceUrl))
                {
                    throw new ArgumentException($"Invalid source URL in line #{lineNumber} in sources batch file '{ApplicationInfo.SourcesBatchFileDescriptor}'.", sourceUrlOption.Name);
                }

                if (!lineSourceUrl.IsAbsoluteUri)
                {
                    throw new ArgumentException($"Source URL in line #{lineNumber} is not an absolute URI.", sourceUrlOption.Name);
                }

                sourceUrls.Add(lineSourceUrl);
            }
        }
    }

    if (!Uri.TryCreate(sourceUrlText, UriKind.Absolute, out Uri? sourceUrl))
    {
        throw new ArgumentException($"{sourceUrlOption.Name} is not a valid URI.", sourceUrlOption.Name);
    }

    sourceUrls.Add(sourceUrl);

    string shareLinkText = parserResult.GetValue(shareLinkOption) ?? throw new ArgumentNullException(shareLinkOption.Name, "The share link is invalid.");
    if (!Uri.TryCreate(shareLinkText, UriKind.Absolute, out Uri shareLink))
    {
        throw new ArgumentException($"{shareLinkOption.Name} is not a valid URI.", shareLinkOption.Name);
    }

    string subfolderText = parserResult.GetValue(subfolderOption) ?? throw new ArgumentNullException(subfolderOption.Name, "The subfolder is invalid.");
    DirectoryDescriptor subfolder;
    try
    {
        subfolder = new DirectoryDescriptor(subfolderText);
    }
    catch (ArgumentException ex)
    {
        throw new ArgumentException($"{subfolderOption.Name} is not a valid directory path.", subfolderOption.Name, ex);
    }

    bool isOverwriteEnabled = parserResult.GetValue(isOverwriteEnabledOption);
    string password = parserResult.GetValue(passwordOption) ?? string.Empty;
    var uploadCommand = new UploadCommand(sourceUrls, sourceUrls.Count, shareLink, subfolder, isOverwriteEnabled, password);
    await uploadCommand.ExecuteAsync(CancellationToken.None);
});

var rootCommand = new RootCommand("Nextcloud URL Uploader")
{
    Subcommands = { uploadCommand },
    TreatUnmatchedTokensAsErrors = true
};

Console.WriteLine(sourceUrls.Count > 0 ? $"Uploading {sourceUrls.Count} files..." : "Uploading file...");
ParseResult parserResult = rootCommand.Parse(args);
await parserResult.InvokeAsync();
Console.WriteLine("Upload completed.");
