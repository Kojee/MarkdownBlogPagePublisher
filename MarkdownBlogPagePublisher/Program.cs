using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CommandLine;
using MarkdownBlogPagePublisher.CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MarkdownBlogPagePublisher
{
    class Program
    {
        static void Main(string[] args)
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

            Parser.Default.ParseArguments<PublishAzure>(args)
                .WithParsed<PublishAzure>(async options => await PublishToAzure(options));
        }

        private async static Task PublishToAzure(PublishAzure options)
        {
            var credentials = new InteractiveBrowserCredential();
            var storageUri = new Uri($"https://{options.AzureStorageName}.blob.core.windows.net");
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageUri, credentials);
            var blobContainer = blobServiceClient.GetBlobContainerClient(options.AzureContainerName);
            if(!(await blobContainer.ExistsAsync()))
            {
                await blobContainer.CreateAsync();
                List<FileInfo> files = GetFilesToUpload(options.InputFolderPath);
                var uploadTasks = new List<Task<string>>();
                foreach (var file in files)
                    uploadTasks.Add(UploadFile(blobContainer, file));
                await Task.WhenAll(uploadTasks);
                var downloadLinks = uploadTasks.Select(t => t.Result);
                CreateMarkdownFile(downloadLinks, options.OutputFilePath);
            }
            else
            {
                //Show error to user
            }
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

        private async static Task<string> UploadFile(BlobContainerClient blobContainer, FileInfo file)
        {
            using (var stream = new FileStream(file.FullName, FileMode.Open))
            {
                var client = blobContainer.GetBlobClient(file.Name);
                await client.UploadAsync(stream);
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
