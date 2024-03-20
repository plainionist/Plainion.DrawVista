
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.Adapters;

public partial class SvgCaptionParser : ISvgCaptionParser
{
    public IReadOnlyCollection<Caption> Parse(XElement document) =>
        document
            .Descendants()
            .Where(x => x.IsMostInnerDiv())
            .Select(x => new Caption(x, GetDisplayText(x.Value)))
            .ToList();

    private static string GetDisplayText(string value) =>
        WhitespacePattern().Replace(value, "")
            .Replace("<br/>", "");
    
    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespacePattern();
}