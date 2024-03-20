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
        static bool EqualsTagName(XElement element, string name) =>
            element.Name.LocalName.Equals(name, StringComparison.OrdinalIgnoreCase);

        var captions = XElement.Parse(document.Content)
            .Descendants()
            .Where(x => EqualsTagName(x, "div") && !x.Elements().Any(x => EqualsTagName(x, "div")))
            .Select(x => myParser.GetDisplayText(x.Value));

        var matchingCaptions = captions
            .Where(x => x.Contains(text, StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return matchingCaptions.Any() ? new(document.Name, matchingCaptions) : null;
    }
}