using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;

var drawIOFile = args[0];

Console.WriteLine($"Analyzing file: {drawIOFile}");

var diagrams = XElement.Load(drawIOFile)
    .Elements("diagram")
    .ToList();

static byte[] Decompress(byte[] data)
{
    using var decompressedStream = new MemoryStream();
    using var compressStream = new MemoryStream(data);
    using var deflateStream = new DeflateStream(compressStream, CompressionMode.Decompress);

    deflateStream.CopyTo(decompressedStream);
    return decompressedStream.ToArray();
}

static XElement ReadGraphModel(string diagram)
{
    var data = Convert.FromBase64String(diagram);
    var decompressed = Decompress(data);
    var content = Encoding.UTF8.GetString(decompressed);
    var decoded = HttpUtility.UrlDecode(content);
    return XElement.Parse(decoded);
}

var pages = diagrams
    .Select(x => (name: x.Attribute("name").Value, xml: ReadGraphModel(x.Value)))
    .ToList();

void AddLinks(string svgFile)
{
    /*
<foreignObject pointer-events="none" width="100%" height="100%" requiredFeatures="http://www.w3.org/TR/SVG11/feature#Extensibility" style="overflow: visible; text-align: left;">
          <div xmlns="http://www.w3.org/1999/xhtml" style="display: flex; align-items: unsafe center; justify-content: unsafe center; width: 118px; height: 1px; padding-top: 250px; margin-left: 348px;">
            <div style="box-sizing: border-box; font-size: 0px; text-align: center;" onclick="window.hook.navigate('Orchestrator')">
              <div style="display: inline-block; font-size: 12px; font-family: Helvetica;cursor: pointer;  color: blue; text-decoration:underline; line-height: 1.2; pointer-events: all; white-space: normal; overflow-wrap: normal;">Orchestrator</div>
            </div>
          </div>
        </foreignObject>    
    */
    var clickable = XElement.Load(svgFile)
        .Descendants()
        .Where(x => x.Name.LocalName == "div")
        .Select(x => (x, name: Regex.Replace(x.Value, @"\s+", "")))
        .Where(x => pages.Any(p => p.name.Equals(x.name, StringComparison.OrdinalIgnoreCase)))
        .ToList();
    foreach (var element in clickable)
    {
        Console.WriteLine(element);
    }
}

for (int i = 0; i < pages.Count; ++i)
{
    var svgFile = Path.Combine("src", "browser", "src", "assets", pages[i].name + ".svg");

    // Process.Start(
    //     Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "draw.io", "draw.io.exe"),
    //     $"-x {drawIOFile} -o {svgFile} -p {i}"
    // ).WaitForExit();

    AddLinks(svgFile);
}


