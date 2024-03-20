using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.IO;

public class DocumentStore(string RootFolder) : IDocumentStore
{
    private readonly object myLock = new object();

    public IReadOnlyCollection<string> GetPageNames()
    {
        lock (myLock)
        {
            return Directory.GetFiles(RootFolder, "*.svg")
                .Select(Path.GetFileNameWithoutExtension)
                .ToList();
        }
    }

    public RawDocument GetPage(string pageName)
    {
        lock (myLock)
        {
            return new RawDocument(pageName, File.ReadAllText(Path.Combine(RootFolder, pageName + ".svg")));
        }
    }

    public void Save(ProcessedDocument document)
    {
        lock (myLock)
        {
            var svgFile = Path.Combine(RootFolder, document.Name + ".svg");
            File.WriteAllText(svgFile, document.Content);
        }
    }

    public void Clear()
    {
        lock (myLock)
        {
            foreach (var file in Directory.GetFiles(RootFolder))
            {
                File.Delete(file);
            }
        }
    }
}
