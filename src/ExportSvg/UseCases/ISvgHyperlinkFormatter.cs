using System.Xml.Linq;

namespace ExportSVG.UseCases;

public interface ISvgHyperlinkFormatter
{
    void ApplyStyle(XElement xml);
}