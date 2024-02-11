using ExportSVG.Adapters;
using ExportSVG.IO;
using ExportSVG.UseCases;

namespace ExportSVG;

public class ProgramFiles
{
    public static void Main(string[] args)
    {
        var drawIOFile = args[0];

        Console.WriteLine($"Analyzing file: {drawIOFile}");

        var outputFolder = Path.Combine("src", "browser", "src", "assets");
        var drawIoWorkbook = new DrawIOWorkbook(drawIOFile, outputFolder);

        var svgProcessor = new SvgProcessor(
            new SvgCaptionParser(),
            new SvgHyperlinkFormatter());

        svgProcessor.Process(drawIoWorkbook);
    }
}
