using System.Xml.Linq;
using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.IO;

public class DrawIOWorkbook(string RootFolder) : IDrawingWorkbook
{
    public IReadOnlyCollection<SvgDocument> Load(string name, Stream stream)
    {
        Console.WriteLine($"DrawIOWorkbook.Load({name})");

        using var reader = new StreamReader(stream);

        var content = reader.ReadToEnd();

        // this file is needed temporarily only for the export of SVG
        var file = Path.Combine(RootFolder, name);

        try
        {
            File.WriteAllText(file, content);

            return GetPageNames(content)
                .Select((name, idx) => Export(file, idx, name))
                .ToList();
        }
        finally
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }
    }

    private static List<string> GetPageNames(string content) =>
        XElement.Parse(content)
            .Elements("diagram")
            .Select(x => x.Attribute("name").Value)
            .ToList();

    private SvgDocument Export(string file, int idx, string name)
    {
        // we want to keep this file
        var svgFile = Path.Combine(RootFolder, name + ".svg");

        DrawIOApp.ExtractSvg(file, svgFile, idx);

        return new SvgDocument(name, XElement.Load(svgFile));
    }
}
