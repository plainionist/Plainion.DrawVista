
using System.Text.RegularExpressions;

namespace ExportSVG.Adapters;

public class SvgCaptionParser
{
    public string GetDisplayText(string value) =>
        Regex.Replace(value, @"\s+", "")
            .ToLower()
            .Replace("<br/>", "");
}