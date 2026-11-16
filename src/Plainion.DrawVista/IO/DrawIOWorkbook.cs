using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.IO;

public class DrawIOWorkbook(string RootFolder, string Name) : IDrawingWorkbook
{
    public IReadOnlyCollection<RawDocument> Load(Stream stream)
    {
        Console.WriteLine($"DrawIOWorkbook.Load({Name})");

        var model = ExtractModel(stream);

        using var app = new DrawIOApp(model, RootFolder);

        return model.GetPageNames()
            .Select((name, idx) => new RawDocument(name, app.ExtractSvg(idx)))
            .ToList();
    }

    private static DrawIOModel ExtractModel(Stream stream)
    {
        using var reader = new StreamReader(stream);
        return new DrawIOModel(reader.ReadToEnd());
    }

}
