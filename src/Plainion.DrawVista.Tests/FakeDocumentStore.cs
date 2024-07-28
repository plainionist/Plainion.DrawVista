using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.Tests;

internal class FakeDocumentStore : IDocumentStore
{
    private readonly List<ProcessedDocument> myDocuments = [];

    public event EventHandler DocumentsChanged;

    public ProcessedDocument GetPage(string pageName) =>
        myDocuments.Single(x => x.Name == pageName);

    public IReadOnlyCollection<string> GetPageNames() =>
        myDocuments.Select(x => x.Name).ToList();

    public void Save(ProcessedDocument document) =>
        myDocuments.Add(document);

    public void Clear() => 
        myDocuments.Clear();
}