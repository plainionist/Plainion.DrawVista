
using System.Text.RegularExpressions;
using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.Adapters;

public class SvgCaptionParser : ISvgCaptionParser
{
    public string GetDisplayText(string value) =>
        Regex.Replace(value, @"\s+", "")
            .ToLower()
            .Replace("<br/>", "");
}