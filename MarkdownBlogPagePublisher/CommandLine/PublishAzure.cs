using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownBlogPagePublisher.CommandLine
{
    [Verb("publish-from-azure", HelpText = "Upload images to Azure and create markdown")]
    public class PublishAzure : Options
    {
        [Option(longName: "storage", HelpText = "Name of the azure storage account where to upload the images", Required = true)]
        public string AzureStorageName { get; set; }

        [Option(longName: "container", HelpText = "Name of the container where to upload the images", Required = true)]
        public string AzureContainerName { get; set; }

        [Option(longName: "key", HelpText = "Access key for the specified storage account", Required = true)]
        public string AzureAccessKey { get; set; }

        [Option(longName: "overwrite", Default = false, HelpText = "Overwrites the container content if present")]
        public bool Overwrite { get; set; }
    }
}
