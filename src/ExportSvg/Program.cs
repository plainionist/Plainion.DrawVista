using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Linq;

string DrawIoExecutable = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
     "draw.io", "draw.io.exe");

var drawIOFile = args[0];

Console.WriteLine($"Analyzing file: {drawIOFile}");

var pages = XElement.Load(drawIOFile)
    .Elements("diagram")
    .Select(x => x.Attribute("name").Value)
    .ToList();

string GetRawText(string value) =>
    Regex.Replace(value, @"\s+", "")
        .ToLower()
        .Replace("<br/>", "");

bool IsPageReference(string name) =>
    pages.Any(p => p.Equals(name, StringComparison.OrdinalIgnoreCase));

void AddLinks(string pageName, string svgFile)
{
    var doc = XElement.Load(svgFile);

    var elementsReferencingPages = doc
        .Descendants()
        .Where(x => x.Name.LocalName == "div" && !x.Elements().Any(x => x.Name.LocalName == "div"))
        .Select(x => (xml: x, name: GetRawText(x.Value)))
        .Where(x => IsPageReference(x.name))
        // skip self-references
        .Where(x => !x.name.Equals(pageName, StringComparison.OrdinalIgnoreCase))
        .ToList();

    foreach (var (xml, name) in elementsReferencingPages)
    {
        Console.WriteLine("Creating link for: " + name);

        xml.Add(new XAttribute("onclick", $"window.hook.navigate('{name}')"));

        var attrs = xml.Attribute("style").Value.Split(";")
            .Where(x => !string.IsNullOrEmpty(x))
            .Select(x => x.Split(':')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x)))
            .ToDictionary(x => x.First(), x => x.Last());

        attrs["color"] = "blue";
        attrs["text-decoration"] = "underline";
        attrs["cursor"] = "pointer";

        xml.Attribute("style").Value = string.Join(";", attrs.Select(x => x.Key + ": " + x.Value));
    }

    doc.Attribute("width").Value = "100%";

    File.WriteAllText(svgFile, doc.ToString());
}

for (int i = 0; i < pages.Count; ++i)
{
    var svgFile = Path.Combine("src", "browser", "src", "assets", pages[i] + ".svg");

    Process.Start(DrawIoExecutable, $"-x {drawIOFile} -o {svgFile} -p {i}")
        .WaitForExit();

    AddLinks(pages[i], svgFile);
}
