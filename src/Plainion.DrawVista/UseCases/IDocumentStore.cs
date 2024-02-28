namespace Plainion.DrawVista.UseCases;

public interface IDocumentStore
{
    IReadOnlyCollection<string> GetPageNames();
    RawDocument GetPage(string pageName);
    void Save(SvgDocument document);
    void Clear();
}

public record RawDocument(string Name, string Content);
