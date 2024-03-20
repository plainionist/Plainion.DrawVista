using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.IO;

public class DrawIOWorkbook(string RootFolder, string Name) : IDrawingWorkbook
{
    public IReadOnlyCollection<RawDocument> Load(Stream stream)
    {
        Console.WriteLine($"DrawIOWorkbook.Load({Name})");

        var model = ExtractModel(stream);

        using var app = new DrawIOApp(model);

        return model.GetPageNames()
            .Select((name, idx) => ExportSvg(app, idx, name))
            .ToList();
    }

    private static DrawIOModel ExtractModel(Stream stream)
    {
        using var reader = new StreamReader(stream);
        return new DrawIOModel(reader.ReadToEnd());
    }

    private RawDocument ExportSvg(DrawIOApp app, int idx, string name)
    {
        var svgFile = Path.Combine(RootFolder, name + ".svg");

        app.ExtractSvg(idx, svgFile);

        return new RawDocument(name, File.ReadAllText(svgFile));
    }
}
