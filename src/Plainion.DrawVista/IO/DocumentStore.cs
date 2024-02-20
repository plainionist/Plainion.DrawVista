using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.IO;

public class DocumentStore(string RootFolder) : IDocumentStore
{
    public IReadOnlyCollection<RawDocument> GetAllFiles() =>
        Directory.GetFiles(RootFolder, "*.svg")
            .Select(f => new RawDocument(Path.GetFileNameWithoutExtension(f), File.ReadAllText(f)))
            .ToList();

    public string GetFileName(string pageName) =>
        Path.Combine(RootFolder, pageName + ".svg");

    public void Save(SvgDocument document)
    {
        var svgFile = Path.Combine(RootFolder, document.Name + ".svg");
        File.WriteAllText(svgFile, document.Content.ToString());
    }

    public void Clear()
    {
        foreach (var file in Directory.GetFiles(RootFolder))
        {
            File.Delete(file);
        }
    }
}
