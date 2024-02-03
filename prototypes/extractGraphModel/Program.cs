using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;

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

void AddLinks(string page, string svgFile)
{
    var doc = XElement.Load(svgFile);

    string GetName(string value) =>
        Regex.Replace(value, @"\s+", "").ToLower().Replace("<br/>", "");

    bool IsOtherPage(string name) =>
        !name.Equals(page, StringComparison.OrdinalIgnoreCase)
            && pages.Any(p => p.name.Equals(name, StringComparison.OrdinalIgnoreCase));

    var clickable = doc
        .Descendants()
        .Where(x => x.Name.LocalName == "div" && !x.Elements().Any(x => x.Name.LocalName == "div"))
        .Select(x => (xml: x, name: GetName(x.Value)))
        .Where(x => IsOtherPage(x.name))
        .ToList();

    foreach (var element in clickable)
    {
        Console.WriteLine("Creating link for: " + element.name);

        element.xml.Add(new XAttribute("onclick", $"window.hook.navigate('{element.name}')"));

        var attrs = element.xml.Attribute("style").Value.Split(";")
            .Where(x => !string.IsNullOrEmpty(x))
            .Select(x => x.Split(':')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x)))
            .ToDictionary(x => x.First(), x => x.Last());
        attrs["color"] = "blue";
        attrs["text-decoration"] = "underline";
        attrs["cursor"] = "pointer";
        element.xml.Attribute("style").Value = string.Join(";", attrs.Select(x => x.Key + ": " + x.Value));
    }

    doc.Attribute("width").Value="100%";
    
    File.WriteAllText(svgFile, doc.ToString());
}

for (int i = 0; i < pages.Count; ++i)
{
    var svgFile = Path.Combine("src", "browser", "src", "assets", pages[i].name + ".svg");

    Process.Start(
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "draw.io", "draw.io.exe"),
        $"-x {drawIOFile} -o {svgFile} -p {i}"
    ).WaitForExit();

    AddLinks(pages[i].name, svgFile);
}


