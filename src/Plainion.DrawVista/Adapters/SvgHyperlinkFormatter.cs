using System.Xml.Linq;
using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.Adapters;

public class SvgHyperlinkFormatter : ISvgHyperlinkFormatter
{
    public void ApplyStyle(XElement xml, bool isExternal)
    {
        var styleAddr = xml.Attribute("style");
        if (styleAddr == null)
        {
            styleAddr = new XAttribute("style", "");
            xml.Add(styleAddr);
        }

        var attr = new SvgStyleAttribute(styleAddr.Value);

        attr["color"] = "blue";
        attr["text-decoration"] = "underline";
        if (isExternal)
        {
            attr["text-decoration-style"] = "dotted";
        }
        attr["cursor"] = "pointer";

        xml.Attribute("style").Value = attr.ToString();
    }
}
