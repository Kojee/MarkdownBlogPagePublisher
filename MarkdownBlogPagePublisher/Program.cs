using CommandLine;
using MarkdownBlogPagePublisher.CommandLine;
using System;

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
                .WithParsed<PublishAzure>(options => PublishToAzure(options));
        }

        private static void PublishToAzure(PublishAzure options)
        {
            throw new NotImplementedException();
        }
    }
}
