using System.CommandLine;
using BionicCode.Utilities.Net;
using NextcloudUrlUploader;

Console.WriteLine("Hello, World!");

// TODO::Support batch upload of files from a list (file) of source URLs to a Nextcloud share link.
var sourceUrlOption = new Option<Uri>("--sourceUrl", "--su", "source")
{
    Description = "The URL of the file to upload.",
    Required = true,
};
var shareLinkOption = new Option<Uri>("--shareLink", "--sl", "target")
{
    Description = "The share link to upload files to.",
    Required = true,
};
var subfolderOption = new Option<DirectoryDescriptor>("--subfolder", "--sf", "--subdir")
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
    sourceUrlOption,
    shareLinkOption,
    subfolderOption,
    isOverwriteEnabledOption,
    passwordOption
};
uploadCommand.SetAction(async (parserResult) =>
{
    Uri sourceUrl = parserResult.GetValue(sourceUrlOption) ?? throw new ArgumentNullException("--sourceUrl", "The source URL cannot be null.");
    Uri shareLink = parserResult.GetValue(shareLinkOption) ?? throw new ArgumentNullException("--shareLink", "The share link cannot be null.");
    DirectoryDescriptor subfolder = parserResult.GetValue(subfolderOption);
    bool isOverwriteEnabled = parserResult.GetValue(isOverwriteEnabledOption);
    string password = parserResult.GetValue(passwordOption) ?? string.Empty;
    var uploadCommand = new UploadCommand(sourceUrl, shareLink, subfolder, isOverwriteEnabled, password);
    await uploadCommand.ExecuteAsync();
});

var rootCommand = new RootCommand("Nextcloud URL Uploader")
{
    uploadCommand
};

ParseResult parserResult = rootCommand.Parse(args);
await parserResult.InvokeAsync();
