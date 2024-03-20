using System.Xml.Linq;

namespace Plainion.DrawVista.UseCases;

public record SearchResult(string PageName, IReadOnlyCollection<string> Captions);

public class FullTextSearch(IDocumentStore store, ISvgCaptionParser parser)
{
    private readonly IDocumentStore myStore = store;
    private readonly ISvgCaptionParser myParser = parser;

    public IReadOnlyCollection<SearchResult> Search(string text) =>
        myStore.GetPageNames()
            .Select(myStore.GetPage)
            .Select(x => Search(x, text))
            .Where(x => x != null)
            .ToList();

    private SearchResult Search(RawDocument document, string text)
    {
        var captions = myParser.Parse(XElement.Parse(document.Content));

        var matchingCaptions = captions
            .Select(x => x.DisplayText)
            .Where(x => x.Contains(text, StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return matchingCaptions.Any() ? new(document.Name, matchingCaptions) : null;
    }
}