
using System.Diagnostics;
using System.Xml.Linq;
using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.IO;

public class DrawIOWorkbook(string RootFolder, string Name) : IDrawingWorkbook
{
    private readonly string DrawIoExecutable = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
         "draw.io", "draw.io.exe");

    private IReadOnlyList<string> ReadPages(string file)
    {
        return XElement.Load(file)
            .Elements("diagram")
            .Select(x => x.Attribute("name").Value)
            .ToList();
    }

    private SvgDocument Export(string file, int pageIndex, string pageName)
    {
        var svgFile = Path.Combine(RootFolder, pageName + ".svg");

        Process.Start(DrawIoExecutable, $"-x {file} -o {svgFile} -p {pageIndex}")
            .WaitForExit();

        return new SvgDocument(pageName, XElement.Load(svgFile));
    }

    public IReadOnlyCollection<SvgDocument> Load(Stream stream)
    {
        Console.WriteLine($"DrawIOPngWorkbook.Load({Name})");

        var tempFile = Path.GetTempFileName();
        try
        {
            using (var output = File.OpenWrite(tempFile))
            {
                stream.CopyTo(output);
            }

            return ReadPages(tempFile)
                .Select((p, idx) => Export(tempFile, idx, p))
                .ToList();
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }
}
