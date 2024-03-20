using System.Xml.Linq;

namespace Plainion.DrawVista.UseCases;

public interface ISvgCaptionParser
{
    IReadOnlyCollection<Caption> Parse(XElement document);
}

public record Caption(XElement Element, string DisplayText);

