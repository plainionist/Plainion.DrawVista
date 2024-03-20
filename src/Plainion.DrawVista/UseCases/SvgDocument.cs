using System.Xml.Linq;

namespace Plainion.DrawVista.UseCases;

public record SvgDocument(string Name, XElement Content, IReadOnlyCollection<string> Captions);
