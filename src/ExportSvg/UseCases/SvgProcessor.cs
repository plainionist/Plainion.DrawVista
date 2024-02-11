using System.Xml.Linq;

namespace ExportSVG.UseCases;

public class SvgProcessor
{
    private readonly ISvgCaptionParser myParser;
    private readonly ISvgHyperlinkFormatter myFormatter;

    public SvgProcessor(ISvgCaptionParser parser, ISvgHyperlinkFormatter formatter)
    {
        myParser = parser;
        myFormatter = formatter;
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

    public void Process(IDrawingWorkbook workbook)
    {
        var pages = workbook.ReadPages();
        for (int i = 0; i < pages.Count; ++i)
        {
            var svgDocument = workbook.Export(i, pages[i]);

            AddLinks(pages, svgDocument);

            workbook.Save(svgDocument);
        }
    }
}
