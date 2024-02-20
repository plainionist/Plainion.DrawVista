namespace Plainion.DrawVista.UseCases;

public interface IDocumentStore
{
    IReadOnlyCollection<RawDocument> GetAllFiles();
    string GetFileName(string pageName);
    void Save(SvgDocument document);
    void Clear();
}

public record RawDocument(string Name, string Content);
