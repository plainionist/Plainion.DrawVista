
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.Adapters;

public partial class SvgCaptionParser : ISvgCaptionParser
{
    public IReadOnlyCollection<Caption> Parse(XElement document) =>
        document
            .Descendants()
            .Where(x => EqualsTagName(x, "div") && !x.Elements().Any(x => EqualsTagName(x, "div")))
            .Select(x => new Caption(x, GetDisplayText(x.Value)))
            .ToList();

    private static bool EqualsTagName(XElement element, string name) =>
        element.Name.LocalName.Equals(name, StringComparison.OrdinalIgnoreCase);

    private static string GetDisplayText(string value) =>
        WhitespacePattern().Replace(value, "")
            .Replace("<br/>", "");
    
    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespacePattern();
}