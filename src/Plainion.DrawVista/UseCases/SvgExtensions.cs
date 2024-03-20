using System.Xml.Linq;

namespace Plainion.DrawVista.UseCases;

static class SvgExtensions
{
    public static bool EqualsTagName(this XElement self, string name) =>
        self.Name.LocalName.Equals(name, StringComparison.OrdinalIgnoreCase);

    public static bool IsMostInnerDiv(this XElement self)=>
        self.EqualsTagName("div") && !self.Elements().Any(x => x.EqualsTagName("div"));
}
