using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Linq;

var drawIOFile = args[0];

Console.WriteLine($"Analyzing file: {drawIOFile}");

var pages = XElement.Load(drawIOFile)
    .Elements("diagram")
    .Select(x => x.Attribute("name").Value)
    .ToList();

string GetName(string value) =>
    Regex.Replace(value, @"\s+", "").ToLower().Replace("<br/>", "");

bool IsOtherPage(string page, string name) =>
    !name.Equals(page, StringComparison.OrdinalIgnoreCase)
        && pages.Any(p => p.Equals(name, StringComparison.OrdinalIgnoreCase));

void AddLinks(string pageName, string svgFile)
{
    var doc = XElement.Load(svgFile);

    var elementsReferencingPages = doc
        .Descendants()
        .Where(x => x.Name.LocalName == "div" && !x.Elements().Any(x => x.Name.LocalName == "div"))
        .Select(x => (xml: x, name: GetName(x.Value)))
        .Where(x => IsOtherPage(pageName, x.name))
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

    Process.Start(
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "draw.io", "draw.io.exe"),
        $"-x {drawIOFile} -o {svgFile} -p {i}"
    ).WaitForExit();

    AddLinks(pages[i], svgFile);
}
