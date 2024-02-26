using System.Xml.Linq;
using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.IO;

public class DrawIOWorkbook(string RootFolder, string Name) : IDrawingWorkbook
{
    public async Task<IReadOnlyCollection<SvgDocument>> LoadAsync(Stream stream)
    {
        Console.WriteLine($"DrawIOWorkbook.Load({Name})");

        var model = await ExtractModelAsync(stream);

        using var app = new DrawIOApp(model);

        var tasks =  model.GetPageNames()
            .Select((name, idx) => ExportSvg(app, idx, name))
            .ToList();

        return await Task.WhenAll(tasks);
    }

    private static async Task<DrawIOModel> ExtractModelAsync(Stream stream)
    {
        using var reader = new StreamReader(stream);
        return new DrawIOModel(await reader.ReadToEndAsync());
    }

    private async Task<SvgDocument> ExportSvg(DrawIOApp app, int idx, string name)
    {
        var svgFile = Path.Combine(RootFolder, name + ".svg");

        app.ExtractSvg(idx, svgFile);

        using var stream = File.OpenRead(svgFile);
        return new SvgDocument(name, await XElement.LoadAsync(stream, LoadOptions.None, default));
    }
}
