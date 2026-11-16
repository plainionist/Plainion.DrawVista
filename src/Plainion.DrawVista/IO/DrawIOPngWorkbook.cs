using System.Web;
using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.IO;

public class DrawIOPngWorkbook(string RootFolder, string Name) : IDrawingWorkbook
{
    public IReadOnlyCollection<RawDocument> Load(Stream stream)
    {
        Console.WriteLine($"DrawIOPngWorkbook.Load({Name})");

        var model = ExtractModel(stream);

        return [ExportSvg(Name, model)];
    }

    private static DrawIOModel ExtractModel(Stream stream)
    {
        var tag = MetadataExtractor.ImageMetadataReader.ReadMetadata(stream)
            .SelectMany(x => x.Tags)
            .SingleOrDefault(x => x.DirectoryName == "PNG-tEXt" && x.Name == "Textual Data"
                && x.Description.StartsWith("mxfile: ", StringComparison.Ordinal))
            ?? throw new InvalidDataException("PNG contains no embedded draw.io model.");

        var xml = HttpUtility.UrlDecode(tag.Description).Substring("mxfile: ".Length);
        return new DrawIOModel(xml);
    }

    private RawDocument ExportSvg(string name, DrawIOModel model)
    {
        var pageName = Path.GetFileNameWithoutExtension(name);
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

        using var app = new DrawIOApp(model, RootFolder);
        return new RawDocument(pageName, app.ExtractSvg(pageIndex));
    }
}
