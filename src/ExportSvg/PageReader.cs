
using System.Xml.Linq;

internal class PageReader
{
    private readonly string myDrawIOFile;

    public PageReader(string drawIOFile)
    {
        myDrawIOFile = drawIOFile;
    }

    internal IReadOnlyList<string> ReadPages()
    {
        return XElement.Load(myDrawIOFile)
            .Elements("diagram")
            .Select(x => x.Attribute("name").Value)
            .ToList();
    }
}
