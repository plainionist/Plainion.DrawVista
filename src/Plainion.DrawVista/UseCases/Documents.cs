
namespace Plainion.DrawVista.UseCases;

public record RawDocument(string Name, string Content);

public record ProcessedDocument(string Name, string Content, IReadOnlyCollection<string> Captions);
