using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.IO;

public class DocumentStore(string RootFolder) : IDocumentStore
{
    public void Save(SvgDocument document)
    {
        var svgFile = Path.Combine(RootFolder, document.Name + ".svg");
        File.WriteAllText(svgFile, document.Content.ToString());
    }
}
