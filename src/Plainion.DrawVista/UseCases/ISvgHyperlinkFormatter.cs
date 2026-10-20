using System.Xml.Linq;

namespace Plainion.DrawVista.UseCases;

public interface ISvgHyperlinkFormatter
{
    void ApplyStyle(XElement xml, bool isExternal);
    void RemoveStyle(XElement xml);
}