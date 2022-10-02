#nullable disable

namespace Egrep_Cpy.Log;

using CommandLine;

public class CommandLineOptions
{

    [Option('r', "regex", Required = true, HelpText = "The regular expression to use for matching.")]
    public string RegEx { get; set; }

    [Option('f', "file", Required = true, HelpText = "The file to search in.")]
    public string File { get; set; }

    [Option('d', "detail", Required = false, Default = false, HelpText = "Shows a detailed output with the RegEx tree, the NFA and the DFA results.")]
    public bool PrintDetails { get; set; }

    [Option('n', "number", Required = false, Default = false, HelpText = "Shows the total number of matches found.")]
    public bool PrintCount { get; set; }

    //Line by line options
    [Option('c', "col-number", SetName = "Line by line", Required = false, Default = false, HelpText = "Prefix each line of output with the 2-based (start and end) colmun number within its input file.")]
    public bool PrintRange { get; set; }

    [Option('l', "line-number", SetName = "Line by line", Required = false, Default = false, HelpText = "Prefix each line of output with the 1-based line number within its input file.")]
    public bool PrintLine { get; set; }

    //Pretty print options
    [Option('p', "pretty", SetName = "Pretty print", Required = false, Default = false, HelpText = "Shows the matches in a colorful way with the matched text in green.")]
    public bool PrettyPrint { get; set; }
}