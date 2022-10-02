#nullable disable

namespace Egrep_Cpy.Log;

using CommandLine;

public class CommandLineOptions
{

    [Option('r', "regex", Required = true, HelpText = "The regular expression to use for matching.")]
    public string RegEx { get; set; }

    [Option('f', "file", Required = true, HelpText = "The file to search in.")]
    public string File { get; set; }

    [Option('l', "col-number", Required = false, Default = false, HelpText = "Prefix each line of output with the 2-based (start and end) colmun number within its input file.")] 
    public bool PrintRange { get; set; }

    [Option('n', "line-number", Required = false, Default = false, HelpText = "Prefix each line of output with the 1-based line number within its input file.")]
    public bool PrintLine { get; set; }

    [Option('d', "detail", Required = false, Default = false, HelpText = "Shows a detailed output with the RegEx tree, the NFA and the DFA results.")]
    public bool PrintDetails { get; set; }
}