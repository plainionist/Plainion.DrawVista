using System.Xml.Linq;

namespace ExportSVG.UseCases;

public record SvgDocument(string Name, XElement Content);
