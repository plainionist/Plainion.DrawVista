using System.Xml.Linq;

namespace Plainion.DrawVista.IO;

public class DrawIOModel(string content)
{
    private readonly XElement myContent = XElement.Parse(content);

    public IList<string> GetPageNames() =>
        myContent
            .Elements("diagram")
            .Select(x => x.Attribute("name").Value)
            .ToList();

    public void WriteTo(string file) =>
        File.WriteAllText(file, myContent.ToString());
}
