#nullable disable

namespace EgrepCpy.Log;

using CommandLine;

public class CommandLineOptions
{

    [Option('r', "regex", Required = true, HelpText = "The regular expression to use for matching.")]
    public string RegEx { get; set; }

    [Option('f', "file", Required = true, HelpText = "The file to search in.")]
    public string File { get; set; }

    [Option('d', "detail", Required = false, Default = false, HelpText = "Shows a detailed output with the RegEx tree, the NFA and the DFA results or the carry over table depending on the algorithm used.")]
    public bool PrintDetails { get; set; }

    [Option('c', "count", Required = false, Default = false, HelpText = "Shows the total count of matches found.")]
    public bool PrintCount { get; set; }

    [Option('w', "watch", Required = false, Default = false, HelpText = "Shows the time elapsed for parsing and matching the RegEx (excluding the printing to console time).")]
    public bool PrintTime { get; set; }

    [Option('p', "pretty", Required = false, Default = false, HelpText = "Outputs the whole input file with the matches highlighted in green.")]
    public bool PrettyPrint { get; set; }
}