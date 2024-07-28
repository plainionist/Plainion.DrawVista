namespace Plainion.DrawVista.UseCases;

public interface IDocumentStore
{
    event EventHandler DocumentsChanged;
    IReadOnlyCollection<string> GetPageNames();
    ProcessedDocument GetPage(string pageName);
    void Save(ProcessedDocument document);
    void Clear();
}
