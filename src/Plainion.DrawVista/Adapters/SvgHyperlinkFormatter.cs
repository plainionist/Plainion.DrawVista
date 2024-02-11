using System.Xml.Linq;
using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.Adapters;

public class SvgHyperlinkFormatter : ISvgHyperlinkFormatter
{
    public void ApplyStyle(XElement xml)
    {
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
}