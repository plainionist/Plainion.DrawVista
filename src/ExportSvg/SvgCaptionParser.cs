
using System.Text.RegularExpressions;

public class SvgCaptionParser
{
    public string GetDisplayText(string value) =>
        Regex.Replace(value, @"\s+", "")
            .ToLower()
            .Replace("<br/>", "");
}