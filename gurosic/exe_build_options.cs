using CommandLine;
using CommandLine.Text;

namespace Gurosic;

public sealed class ExeBuildOptions
{
    [Option('t', "target", Required = true)]
    public string Target { get; set; }
    [Option('o', "output", Required = true)]
    public string OutputFileName { get; set; }
    [Option('r', "recurse")]
    public bool Recurse { get; set; }
    [Option('w', "wildcard")]
    public string Wildcard { get; set; } = "*.gr";
}