
using System.Diagnostics;
using System.Xml.Linq;
using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.IO;

public class DrawIOWorkbook(string RootFolder, string Name) : IDrawingWorkbook
{
    public IReadOnlyCollection<SvgDocument> Load(Stream stream)
    {
        Console.WriteLine($"DrawIOPngWorkbook.Load({Name})");

        var model = ExtractModel(stream);

        using var app = new DrawIOApp(model);

        return model.GetPageNames()
            .Select((p, idx) => ExportSvg(app, idx, p))
            .ToList();
    }

    private DrawIOModel ExtractModel(Stream stream)
    {
        using var reader = new StreamReader(stream);
        return new DrawIOModel(reader.ReadToEnd());
    }

    private SvgDocument ExportSvg(DrawIOApp app, int pageIndex, string pageName)
    {
        var svgFile = Path.Combine(RootFolder, pageName + ".svg");

        app.ExtractSvg(pageIndex,svgFile);

        return new SvgDocument(pageName, XElement.Load(svgFile));
    }
}
