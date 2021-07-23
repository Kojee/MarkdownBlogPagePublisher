using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CommandLine;
using MarkdownBlogPagePublisher.CommandLine;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MarkdownBlogPagePublisher
{
    class Program
    {
        static async Task Main(string[] args)
        {
            /*
             * 1) INPUT: input folder, output file path, azure/local publication flag
             *      1.1) if azure: pass storage name and container name
             *      1.2) if local: pass output folder path for images
             * 2) azure:
             *      1) interactive login
             *      2) container creation
             *      3) upload with anonymous access enabled
             *      4) return list of direct download links
             * 3) local:
             *      ...
             * 4) markdown file creation
             */
            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Information()
                            .WriteTo.Console()
                            .CreateLogger();

             await Parser.Default.ParseArguments<PublishAzure>(args)
                .WithParsedAsync<PublishAzure>(async options => await PublishToAzure(options));
        }

        private async static Task PublishToAzure(PublishAzure options)
        {
            var credentials = new StorageSharedKeyCredential(options.AzureStorageName, options.AzureAccessKey);
            var storageUri = new Uri($"https://{options.AzureStorageName}.blob.core.windows.net");
            Log.Information("Starting Azure publication on storage {Storage}{Container} of folder {Folder} - Overwrite {Overwrite}",
                storageUri, options.AzureContainerName, options.InputFolderPath, options.Overwrite);
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageUri, credentials);
            var blobContainer = blobServiceClient.GetBlobContainerClient(options.AzureContainerName);
            await blobContainer.CreateIfNotExistsAsync(PublicAccessType.Blob);
            List<FileInfo> files = GetFilesToUpload(options.InputFolderPath);
            Log.Information("Uploading {Count} files", files.Count);
            var uploadTasks = new List<Task<string>>();
            foreach (var file in files)
                uploadTasks.Add(UploadFile(blobContainer, file, options.Overwrite));
            await Task.WhenAll(uploadTasks);
            Log.Information("Upload done");
            var downloadLinks = uploadTasks.Select(t => t.Result);
            CreateMarkdownFile(downloadLinks, options.OutputFilePath);
        }

        private static void CreateMarkdownFile(IEnumerable<string> downloadLinks, string outputFilePath)
        {
            using (var io = new StreamWriter(outputFilePath))
            {
                foreach (var link in downloadLinks)
                {
                    io.WriteLine($"<Image src=\"{link}\" quality={{90}}/>");
                    io.WriteLine($"<br>");
                }
            }
        }

        private async static Task<string> UploadFile(BlobContainerClient blobContainer, FileInfo file, bool overwrite)
        {
            var client = blobContainer.GetBlobClient(file.Name);
            if (overwrite || !(await client.ExistsAsync()))
            {
                await client.UploadAsync(file.FullName, overwrite);
                await client.SetHttpHeadersAsync(new BlobHttpHeaders() { ContentType = $"image/{file.Extension.Replace(".", "")}" });
                return client.Uri.ToString();
            }else
            {
                return client.Uri.ToString();
            }
        }

        private static List<FileInfo> GetFilesToUpload(string inputFolderPath)
        {
            var folder = new DirectoryInfo(inputFolderPath);
            return folder.GetFiles().ToList();
        }
    }
}
