using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownBlogPagePublisher.CommandLine
{
    public class Options
    {
        [Option(shortName: 'i', longName: "input", HelpText = "Input folder where to retrieve the images for the markdown page", Required = true)]
        public string InputFolderPath { get; set; }
        [Option(shortName: 'o', longName: "output", HelpText = "Full filename for the final markdown page")]
        public string OutputFilePath { get; set; }
    }
}
