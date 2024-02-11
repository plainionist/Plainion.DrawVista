
using System.Diagnostics;
using System.Xml.Linq;
using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.IO;

public class DrawIOWorkbook : IDrawingWorkbook
{
    private readonly string DrawIoExecutable = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
         "draw.io", "draw.io.exe");

    private readonly string myDrawIOFile;
    private readonly string myOutputFolder;

    public DrawIOWorkbook(string drawIOFile, string outputFolder)
    {
        myDrawIOFile = drawIOFile;
        myOutputFolder = outputFolder;
    }

    public IReadOnlyList<string> ReadPages()
    {
        return XElement.Load(myDrawIOFile)
            .Elements("diagram")
            .Select(x => x.Attribute("name").Value)
            .ToList();
    }

    public SvgDocument Export(int pageIndex, string pageName)
    {
        var svgFile = Path.Combine(myOutputFolder, pageName + ".svg");

        Process.Start(DrawIoExecutable, $"-x {myDrawIOFile} -o {svgFile} -p {pageIndex}")
            .WaitForExit();

        return new SvgDocument(pageName, XElement.Load(svgFile));
    }

    public void Save(SvgDocument document)
    {
        var svgFile = Path.Combine(myOutputFolder, document.Name + ".svg");
        File.WriteAllText(svgFile, document.Content.ToString());
    }
}
