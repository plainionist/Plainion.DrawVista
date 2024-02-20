
using System.Diagnostics;
using System.Web;
using System.Xml.Linq;
using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.IO;

public class DrawIOPngWorkbook(string RootFolder, string Name) : IDrawingWorkbook
{
    public IReadOnlyCollection<SvgDocument> Load(Stream stream)
    {
        Console.WriteLine($"DrawIOPngWorkbook.Load({Name})");

        var model = ExtractModel(stream);

        var pageName = Path.GetFileNameWithoutExtension(Name);
        if (pageName.EndsWith(".drawio", StringComparison.OrdinalIgnoreCase))
        {
            pageName = Path.GetFileNameWithoutExtension(pageName);
        }

        var pageIndex = model.GetPageNames().IndexOf(pageName);
        if (pageIndex == -1)
        {
            // TODO: should rather inform the user
            pageIndex = 0;
        }

        using var app = new DrawIOApp(model);

        var svgFile = Path.Combine(RootFolder, pageName + ".svg");
        app.ExtractSvg(pageIndex, svgFile);

        return [new SvgDocument(pageName, XElement.Load(svgFile))];
    }

    private DrawIOModel ExtractModel(Stream stream)
    {
        var tag = MetadataExtractor.ImageMetadataReader.ReadMetadata(stream)
            .SelectMany(x => x.Tags)
            .Where(x => x.DirectoryName == "PNG-tEXt" && x.Name == "Textual Data")
            .Single();

        var xml = HttpUtility.UrlDecode(tag.Description).Substring("mxfile: ".Length);
        return new DrawIOModel(xml);
    }
}
