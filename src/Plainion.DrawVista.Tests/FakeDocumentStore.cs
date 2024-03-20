using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.Tests;

internal class FakeDocumentStore : IDocumentStore
{
    private readonly List<RawDocument> myDocuments = [];

    public RawDocument GetPage(string pageName) =>
        myDocuments.Single(x => x.Name == pageName);

    public IReadOnlyCollection<string> GetPageNames() =>
        myDocuments.Select(x => x.Name).ToList();

    public void Save(SvgDocument document) =>
        Save(new RawDocument(document.Name, document.Content.ToString()));

    public void Save(RawDocument document) =>
        myDocuments.Add(document);

    public void Clear() => 
        myDocuments.Clear();
}