namespace Plainion.DrawVista.UseCases;

public interface IDocumentStore
{
    IReadOnlyCollection<string> GetPageNames();
    ProcessedDocument GetPage(string pageName);
    void Save(ProcessedDocument document);
    void Clear();
}
