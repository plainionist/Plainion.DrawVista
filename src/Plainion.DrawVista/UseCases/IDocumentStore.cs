namespace Plainion.DrawVista.UseCases;

public interface IDocumentStore
{
    event EventHandler DocumentsChanged;
    IReadOnlyCollection<string> GetPageNames();
    ProcessedDocument GetPage(string pageName);
    void Save(ProcessedDocument document);
    bool Delete(string pageName);
    void Clear();
}
