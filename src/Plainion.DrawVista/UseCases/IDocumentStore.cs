namespace Plainion.DrawVista.UseCases;

public interface IDocumentStore
{
    IReadOnlyCollection<string> GetPageNames();
    RawDocument GetPage(string pageName);
    void Save(ProcessedDocument document);
    void Clear();
}
