using System.Xml.Linq;

namespace Plainion.DrawVista.IO;

internal class DrawIOModel(string content)
{
    private readonly XElement myContent = XElement.Parse(content);

    public IList<string> GetPageNames() =>
        myContent
            .Elements("diagram")
            .Select(x => x.Attribute("name").Value)
            .ToList();

    internal void WriteTo(string tempFile) =>
        File.WriteAllText(tempFile, myContent.ToString());
}