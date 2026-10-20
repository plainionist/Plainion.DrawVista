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

    public void Save(ProcessedDocument document)
    {
        myDocuments.RemoveAll(x => x.Name == document.Name);
        myDocuments.Add(document);
        DocumentsChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool Delete(string pageName)
    {
        if (myDocuments.RemoveAll(x => x.Name == pageName) == 0)
        {
            return false;
        }

        DocumentsChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public void Clear()
    {
        myDocuments.Clear();
        DocumentsChanged?.Invoke(this, EventArgs.Empty);
    }
}