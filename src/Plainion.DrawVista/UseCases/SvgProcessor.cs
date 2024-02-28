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

    /// <summary>
    /// Processes existing and newly uploaded documents.
    /// </summary>
    public void Process(IReadOnlyCollection<SvgDocument> documents)
    {
        var existingFiles = myStore.GetPageNames()
            .Where(x => !documents.Any(y => y.Name.Equals(x, StringComparison.OrdinalIgnoreCase)))
            .Select(x => SvgDocument.Create(myStore.GetPage(x)))
            .ToList();

        var knownPageNames = documents.Select(x => x.Name)
            .Concat(existingFiles.Select(x => x.Name))
            .ToList();

        foreach (var doc in documents.Concat(existingFiles))
        {
            AddLinks(knownPageNames, doc);
            ApplyStyleToExistingLinks(doc);
            myStore.Save(doc);
        }
    }

    private static bool EqualsTagName(XElement element, string name) =>
        element.Name.LocalName.Equals(name, StringComparison.OrdinalIgnoreCase);

    private void AddLinks(IReadOnlyCollection<string> pages, SvgDocument doc)
    {
        bool IsPageReference(string name) =>
           pages.Any(p => p.Equals(name, StringComparison.OrdinalIgnoreCase));

        var elementsReferencingPages = doc.Content
            .Descendants()
            .Where(x => EqualsTagName(x, "div") && !x.Elements().Any(x => EqualsTagName(x, "div")))
            .Select(x => (xml: x, name: myParser.GetDisplayText(x.Value)))
            .Where(x => IsPageReference(x.name))
            // skip self-references
            .Where(x => !x.name.Equals(doc.Name, StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var (xml, name) in elementsReferencingPages)
        {
            Console.WriteLine("Creating link for: " + name);

            var onClickAttr = xml.Attribute("onclick");
            if (onClickAttr == null)
            {
                onClickAttr = new XAttribute("onclick", string.Empty);
                xml.Add(onClickAttr);
            }
            onClickAttr.Value = $"window.hook.navigate('{name}')";

            myFormatter.ApplyStyle(xml);
        }

        doc.Content.Attribute("width").Value = "100%";
    }

    // In DrawIO external links can be provided but those are neither in draw.io
    // nor in SVG visualized as links (e.g. blue and underlined) - so let's apply some style
    private void ApplyStyleToExistingLinks(SvgDocument doc)
    {
        var existingLinks = doc.Content
            .Descendants()
            .Where(x => EqualsTagName(x, "a"))
            .ToList();

        foreach (var link in existingLinks)
        {
            myFormatter.ApplyStyle(link);
        }
    }
}