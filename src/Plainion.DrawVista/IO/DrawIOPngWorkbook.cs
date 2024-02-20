using System.Web;
using System.Xml.Linq;
using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.IO;

public class DrawIOPngWorkbook(string RootFolder) : IDrawingWorkbook
{
    public IReadOnlyCollection<SvgDocument> Load(string name, Stream stream)
    {
        Console.WriteLine($"DrawIOPngWorkbook.Load({name})");

        var content = ExtractModel(stream);

        return [Export(name, content)];
    }

    private static string ExtractModel(Stream stream)
    {
        var tag = MetadataExtractor.ImageMetadataReader.ReadMetadata(stream)
            .SelectMany(x => x.Tags)
            .Where(x => x.DirectoryName == "PNG-tEXt" && x.Name == "Textual Data")
            .SingleOrDefault();

        return HttpUtility.UrlDecode(tag.Description).Substring("mxfile: ".Length);
    }

    private static List<string> GetPageNames(string content) =>
        XElement.Parse(content)
            .Elements("diagram")
            .Select(x => x.Attribute("name").Value)
            .ToList();

    private SvgDocument Export(string name, string content)
    {
        var pageName = Path.GetFileNameWithoutExtension(name);
        if (pageName.EndsWith(".drawio", StringComparison.OrdinalIgnoreCase))
        {
            pageName = Path.GetFileNameWithoutExtension(pageName);
        }

        // this file is needed temporarily only for the export of SVG
        // extension ".drawio" is important so that draw.io.exe detects file contents properly
        var file = Path.Combine(RootFolder, pageName + ".drawio");
        try
        {
            File.WriteAllText(file, content);

            // we want to keep this file
            var svgFile = Path.Combine(RootFolder, pageName + ".svg");

            var pageIndex = GetPageNames(content).IndexOf(pageName);
            if (pageIndex == -1)
            {
                // TODO: should rather inform the user
                pageIndex = 0;
            }

            DrawIOApp.ExtractSvg(file, svgFile, pageIndex);

            return new SvgDocument(pageName, XElement.Load(svgFile));
        }
        finally
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }
    }
}
