using System.Xml.Linq;

namespace Plainion.DrawVista.UseCases;

public class FullTextSearch(IDocumentStore store, ISvgCaptionParser parser)
{
    private readonly IDocumentStore myStore = store;
    private readonly ISvgCaptionParser myParser = parser;

    public IReadOnlyCollection<string> Search(string text) =>
        myStore.GetPageNames()
            .Select(myStore.GetPage)
            .Where(x => ContainsMatchingCaption(x, text))
            .Select(x => x.Name)
            .ToList();

    private bool ContainsMatchingCaption(RawDocument document, string text)
    {
        static bool EqualsTagName(XElement element, string name) =>
            element.Name.LocalName.Equals(name, StringComparison.OrdinalIgnoreCase);

        var captions = XElement.Parse(document.Content)
            .Descendants()
            .Where(x => EqualsTagName(x, "div") && !x.Elements().Any(x => EqualsTagName(x, "div")))
            .Select(x => myParser.GetDisplayText(x.Value));

        return captions.Any(x => x.Contains(text, StringComparison.OrdinalIgnoreCase));
    }
}