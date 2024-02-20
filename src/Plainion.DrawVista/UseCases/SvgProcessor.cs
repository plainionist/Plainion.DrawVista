using System.Xml.Linq;

namespace Plainion.DrawVista.UseCases;

public class SvgProcessor
{
    private readonly ISvgCaptionParser myParser;
    private readonly ISvgHyperlinkFormatter myFormatter;
    private readonly IDocumentStore myStore;

    public SvgProcessor(ISvgCaptionParser parser, ISvgHyperlinkFormatter formatter, IDocumentStore store)
    {
        myParser = parser;
        myFormatter = formatter;
        myStore = store;
    }

    private void AddLinks(IReadOnlyCollection<string> pages, SvgDocument doc)
    {
        bool IsPageReference(string name) =>
           pages.Any(p => p.Equals(name, StringComparison.OrdinalIgnoreCase));

        var elementsReferencingPages = doc.Content
            .Descendants()
            .Where(x => x.Name.LocalName == "div" && !x.Elements().Any(x => x.Name.LocalName == "div"))
            .Select(x => (xml: x, name: myParser.GetDisplayText(x.Value)))
            .Where(x => IsPageReference(x.name))
            // skip self-references
            .Where(x => !x.name.Equals(doc.Name, StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var (xml, name) in elementsReferencingPages)
        {
            Console.WriteLine("Creating link for: " + name);

            xml.Add(new XAttribute("onclick", $"window.hook.navigate('{name}')"));

            myFormatter.ApplyStyle(xml);
        }

        doc.Content.Attribute("width").Value = "100%";
    }

    public void Process(IReadOnlyCollection<SvgDocument> documents)
    {
        var pageNames = documents.Select(x => x.Name).ToList();
        foreach (var doc in documents)
        {
            AddLinks(pageNames, doc);

            myStore.Save(doc);
        }
    }
}
