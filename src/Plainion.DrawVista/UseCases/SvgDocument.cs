using System.Xml.Linq;

namespace Plainion.DrawVista.UseCases;

public record SvgDocument(string Name, XElement Content)
{
    public static SvgDocument Create(RawDocument doc) =>
        new(doc.Name, XElement.Parse(doc.Content));
}
