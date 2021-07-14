using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownBlogPagePublisher.CommandLine
{
    [Verb("azure", HelpText = "Type of hosting for your images")]
    public class PublishAzure : Options
    {
        [Option(longName: "storage", HelpText = "Azure storage account name where to upload the images", Required = true)]
        public string AzureStorageName { get; set; }
        [Option(longName: "container", HelpText = "Azure storage account name where to upload the images", Required = true)]
        public string AzureContainerName { get; set; }
    }
}
