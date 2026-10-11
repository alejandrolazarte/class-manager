using ClassManager.EmailPreviews;

const string DefaultOutputDirectory = "email-previews-output";

var outputDirectory = Path.GetFullPath(args.FirstOrDefault() ?? DefaultOutputDirectory);
var files = EmailPreviewWriter.Write(outputDirectory, EmailPreviewCatalog.All());
Console.WriteLine($"Wrote {files.Count} email previews to {outputDirectory}");
