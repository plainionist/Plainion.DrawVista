using System.Web;
using System.Xml.Linq;
using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.IO;

public class DrawIOPngWorkbook(string RootFolder, string Name) : IDrawingWorkbook
{
    public async Task<IReadOnlyCollection<SvgDocument>> LoadAsync(Stream stream)
    {
        Console.WriteLine($"DrawIOPngWorkbook.Load({Name})");

        var model = await ExtractModelAsync(stream);

        return [await ExportSvg(Name, model)];
    }

    private static async Task<DrawIOModel> ExtractModelAsync(Stream stream)
    {
        using var memStream = new MemoryStream();
        await stream.CopyToAsync(memStream);
        memStream.Seek(0, SeekOrigin.Begin);

        var tag = MetadataExtractor.ImageMetadataReader.ReadMetadata(memStream)
            .SelectMany(x => x.Tags)
            .Where(x => x.DirectoryName == "PNG-tEXt" && x.Name == "Textual Data")
            .SingleOrDefault();

        var xml = HttpUtility.UrlDecode(tag.Description).Substring("mxfile: ".Length);
        return new DrawIOModel(xml);
    }

    private async Task<SvgDocument> ExportSvg(string name, DrawIOModel model)
    {
        var pageName = Path.GetFileNameWithoutExtension(name);
        if (pageName.EndsWith(".drawio", StringComparison.OrdinalIgnoreCase))
        {
            pageName = Path.GetFileNameWithoutExtension(pageName);
        }

        var svgFile = Path.Combine(RootFolder, pageName + ".svg");

        var pageIndex = model.GetPageNames().IndexOf(pageName);
        if (pageIndex == -1)
        {
            // TODO: should rather inform the user
            pageIndex = 0;
        }

        using var app = new DrawIOApp(model);
        app.ExtractSvg(pageIndex, svgFile);

        using var stream = File.OpenRead(svgFile);
        return new SvgDocument(pageName, await XElement.LoadAsync(stream, LoadOptions.None, default));
    }
}
