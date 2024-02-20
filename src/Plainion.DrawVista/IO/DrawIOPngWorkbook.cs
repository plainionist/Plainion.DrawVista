
using System.Diagnostics;
using System.Web;
using System.Xml.Linq;
using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.IO;

public class DrawIOPngWorkbook(string RootFolder, string Name) : IDrawingWorkbook
{
    private readonly string DrawIoExecutable = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
         "draw.io", "draw.io.exe");

    private List<string> ReadPages(string file) =>
        XElement.Load(file)
            .Elements("diagram")
            .Select(x => x.Attribute("name").Value)
            .ToList();

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
        
        var model = ExtractModel(stream);

        var tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, model);

            var pageName = Path.GetFileNameWithoutExtension(Name);
            if (pageName.EndsWith(".drawio", StringComparison.OrdinalIgnoreCase))
            {
                pageName = Path.GetFileNameWithoutExtension(pageName);
            }

            var pageIndex = ReadPages(tempFile).IndexOf(pageName);
            if (pageIndex == -1)
            {
                // TODO: should rather inform the user
                pageIndex = 0;
            }

            return [Export(tempFile, pageIndex, pageName)];
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    private string ExtractModel(Stream stream)
    {
        var tag = MetadataExtractor.ImageMetadataReader.ReadMetadata(stream)
            .SelectMany(x => x.Tags)
            .Where(x => x.DirectoryName == "PNG-tEXt" && x.Name == "Textual Data")
            .Single();

        var xml = HttpUtility.UrlDecode(tag.Description).Substring("mxfile: ".Length);
        return xml;
    }
}
