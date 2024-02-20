namespace Plainion.DrawVista.UseCases;

public interface IDocumentStore
{
    void Save(SvgDocument document);
}