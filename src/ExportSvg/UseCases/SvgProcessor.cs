using System.Xml.Linq;

namespace ExportSVG.UseCases;

public class SvgProcessor
{
    private readonly IReadOnlyList<string> myPages;
    private readonly ISvgCaptionParser myParser;
    private readonly ISvgHyperlinkFormatter myFormatter;
    private readonly IDrawingWorkbook myWorkbook;

    public SvgProcessor(ISvgCaptionParser parser, ISvgHyperlinkFormatter formatter,
        IDrawingWorkbook workbook)
    {
        myParser = parser;
        myFormatter = formatter;
        myWorkbook = workbook;

        myPages = workbook.ReadPages();
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

    internal void Process()
    {
        for (int i = 0; i < myPages.Count; ++i)
        {
            var svgDocument = myWorkbook.Export(i, myPages[i]);

            AddLinks(svgDocument);

            myWorkbook.Save(svgDocument);
        }
    }
}
