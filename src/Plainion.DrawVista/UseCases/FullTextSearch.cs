
namespace Plainion.DrawVista.UseCases;

public class FullTextSearch
{
    private readonly IDocumentStore myStore;

    public FullTextSearch(IDocumentStore store)
    {
        myStore = store;
    }

    public IReadOnlyCollection<string> Search(string text)
    {
        return new List<string>();
    }
}