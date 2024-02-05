using System.Xml.Linq;

public class SvgProcessor
{
    private readonly IReadOnlyList<string> myPages;
    private readonly SvgCaptionParser myParser;
    private readonly SvgHyperlinkFormatter myFormatter;

    public SvgProcessor(IReadOnlyList<string> pages, SvgCaptionParser parser, SvgHyperlinkFormatter formatter)
    {
        myPages = pages;
        myParser = parser;
        myFormatter = formatter;
    }

    private bool IsPageReference(string name) =>
        myPages.Any(p => p.Equals(name, StringComparison.OrdinalIgnoreCase));

    public void AddLinks(SvgDocument doc)
    {
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
}
