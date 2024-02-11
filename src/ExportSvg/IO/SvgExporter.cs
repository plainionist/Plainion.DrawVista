
using System.Diagnostics;
using System.Xml.Linq;
using ExportSVG.UseCases;

namespace ExportSVG.IO;

internal class SvgExporter
{
    private readonly string DrawIoExecutable = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
         "draw.io", "draw.io.exe");

    private readonly string myDrawIOFile;
    private readonly string myOutputFolder;

    public SvgExporter(string drawIOFile, string outputFolder)
    {
        myDrawIOFile = drawIOFile;
        myOutputFolder = outputFolder;
    }

    internal SvgDocument Export(int pageIndex, string pageName)
    {
        var svgFile = Path.Combine(myOutputFolder, pageName + ".svg");

        Process.Start(DrawIoExecutable, $"-x {myDrawIOFile} -o {svgFile} -p {pageIndex}")
            .WaitForExit();

        return new SvgDocument(pageName, XElement.Load(svgFile));
    }

    internal void Save(SvgDocument document)
    {
        var svgFile = Path.Combine(myOutputFolder, document.Name + ".svg");
        File.WriteAllText(svgFile, document.Content.ToString());
    }
}
