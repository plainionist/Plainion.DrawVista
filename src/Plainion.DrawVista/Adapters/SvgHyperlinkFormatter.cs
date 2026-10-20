using System.Xml.Linq;
using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.Adapters;

public class SvgHyperlinkFormatter : ISvgHyperlinkFormatter
{
    private const string OriginalStyleAttribute = "data-drawvista-original-style";

    public void ApplyStyle(XElement xml, bool isExternal)
    {
        var styleAddr = xml.Attribute("style");
        if (!isExternal && xml.Attribute(OriginalStyleAttribute) == null)
        {
            xml.SetAttributeValue(OriginalStyleAttribute, styleAddr?.Value ?? string.Empty);
        }

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

    public void RemoveStyle(XElement xml)
    {
        var originalStyle = xml.Attribute(OriginalStyleAttribute);
        if (originalStyle != null)
        {
            xml.SetAttributeValue("style", originalStyle.Value.Length == 0 ? null : originalStyle.Value);
            originalStyle.Remove();
            return;
        }

        var style = xml.Attribute("style");
        if (style == null)
        {
            return;
        }

        // Older stored SVGs have no original-style metadata.
        var attr = new SvgStyleAttribute(style.Value);
        attr.Remove("color");
        attr.Remove("text-decoration");
        attr.Remove("cursor");
        style.Value = attr.ToString();
    }
}
