using System.IO.Compression;
using System.Text;
using System.Web;
using System.Xml.Linq;

// draw.io.exe -x input.drawio -o overview.svg -p 1


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
    .Select(x => (name:x.Attribute("name").Value, xml:ReadGraphModel(x.Value)))
    .ToList();

foreach(var page in pages)
{
    Console.WriteLine($"{page.name}");
}
