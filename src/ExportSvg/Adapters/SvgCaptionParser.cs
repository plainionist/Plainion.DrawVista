
using System.Text.RegularExpressions;
using ExportSVG.UseCases;

namespace ExportSVG.Adapters;

public class SvgCaptionParser : ISvgCaptionParser
{
    public string GetDisplayText(string value) =>
        Regex.Replace(value, @"\s+", "")
            .ToLower()
            .Replace("<br/>", "");
}