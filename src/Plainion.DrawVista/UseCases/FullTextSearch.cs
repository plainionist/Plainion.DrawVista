using System.Xml.Linq;

namespace Plainion.DrawVista.UseCases;

public record SearchResult(string PageName, IReadOnlyCollection<string> Captions);

public class FullTextSearch(IDocumentStore store)
{
    private readonly IDocumentStore myStore = store;

    public IReadOnlyCollection<SearchResult> Search(string text) =>
        myStore.GetPageNames()
            .Select(myStore.GetPage)
            .Select(x => Search(x, text))
            .Where(x => x != null)
            .ToList();

    private SearchResult Search(ProcessedDocument document, string text)
    {
        var matchingCaptions = document.Captions
            .Where(x => x.Contains(text, StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return matchingCaptions.Any() ? new(document.Name, matchingCaptions) : null;
    }
}