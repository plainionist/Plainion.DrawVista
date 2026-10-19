using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.Adapters;

public class DocumentStoreCachingDecorator(IDocumentStore impl) : IDocumentStore
{
    private readonly Dictionary<string, ProcessedDocument> myCache = impl
        .GetPageNames()
        .Select(impl.GetPage)
        .ToDictionary(x => x.Name);

    public event EventHandler DocumentsChanged;

    public void Clear()
    {
        impl.Clear();
        myCache.Clear();
        DocumentsChanged?.Invoke(this, EventArgs.Empty);
    }

    public ProcessedDocument GetPage(string pageName) =>
        myCache[pageName];

    public IReadOnlyCollection<string> GetPageNames() =>
        myCache.Keys.ToList();

    public void Save(ProcessedDocument document)
    {
        impl.Save(document);
        myCache[document.Name] = document;
        DocumentsChanged?.Invoke(this, EventArgs.Empty);
    }
}
