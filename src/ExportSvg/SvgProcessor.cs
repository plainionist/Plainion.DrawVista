
using System.Text.RegularExpressions;
using System.Xml.Linq;

public class SvgProcessor
{
    private readonly IReadOnlyList<string> myPages;

    public SvgProcessor(IReadOnlyList<string> pages)
    {
        myPages = pages;
    }

    private string GetDisplayText(string value) =>
        Regex.Replace(value, @"\s+", "")
            .ToLower()
            .Replace("<br/>", "");

    private bool IsPageReference(string name) =>
        myPages.Any(p => p.Equals(name, StringComparison.OrdinalIgnoreCase));

    public void AddLinks(SvgDocument doc)
    {
        var elementsReferencingPages = doc.Content
            .Descendants()
            .Where(x => x.Name.LocalName == "div" && !x.Elements().Any(x => x.Name.LocalName == "div"))
            .Select(x => (xml: x, name: GetDisplayText(x.Value)))
            .Where(x => IsPageReference(x.name))
            // skip self-references
            .Where(x => !x.name.Equals(doc.Name, StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var (xml, name) in elementsReferencingPages)
        {
            Console.WriteLine("Creating link for: " + name);

            xml.Add(new XAttribute("onclick", $"window.hook.navigate('{name}')"));

            var attrs = xml.Attribute("style").Value.Split(";")
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => x.Split(':')
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x)))
                .ToDictionary(x => x.First(), x => x.Last());

            attrs["color"] = "blue";
            attrs["text-decoration"] = "underline";
            attrs["cursor"] = "pointer";

            xml.Attribute("style").Value = string.Join(";", attrs.Select(x => x.Key + ": " + x.Value));
        }

        doc.Content.Attribute("width").Value = "100%";
    }
}
