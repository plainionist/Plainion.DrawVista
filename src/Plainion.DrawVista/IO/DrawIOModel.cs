using System.IO.Compression;
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

    public IReadOnlyList<DrawIOLayer> GetLayers(int pageIndex) =>
        LayerElements(ReadPage(myContent.Elements("diagram").ElementAt(pageIndex)))
            .Select(x => new DrawIOLayer(
                CellId(x),
                (string)x.Attribute("label") ?? (string)Cell(x).Attribute("value") ?? "Background",
                (string)Cell(x).Attribute("visible") != "0"))
            .ToList();

    internal DrawIOModel ForExport()
    {
        var copy = new XElement(myContent);
        foreach (var diagram in copy.Elements("diagram"))
        {
            var graph = ReadPage(diagram);
            foreach (var layer in LayerElements(graph))
            {
                Cell(layer).SetAttributeValue("visible", "1");
            }
            diagram.ReplaceNodes(graph);
        }
        return new DrawIOModel(copy.ToString());
    }

    internal string AddLayerMetadata(int pageIndex, XElement svg)
    {
        var graph = ReadPage(myContent.Elements("diagram").ElementAt(pageIndex));
        var root = GraphRoot(graph);
        var rootId = CellId(root.Elements().Single(x => Cell(x).Attribute("parent") == null));
        var layers = GetLayers(pageIndex);
        var ns = svg.Name.Namespace;
        var group = svg.Descendants(ns + "g")
            .SingleOrDefault(x => (string)x.Attribute("data-cell-id") == rootId)
            ?? throw new InvalidDataException("Draw.io SVG export did not retain the page's cell groups.");

        var exportedGroups = group.Elements().ToList();
        if (exportedGroups.Any(x => x.Name != ns + "g" ||
            !layers.Any(layer => layer.Id == (string)x.Attribute("data-cell-id"))))
        {
            throw new InvalidDataException("Draw.io SVG export contains content outside its layers.");
        }

        var orderedGroups = layers.Select(layer =>
        {
            var layerGroup = exportedGroups
                .SingleOrDefault(x => (string)x.Attribute("data-cell-id") == layer.Id)
                ?? new XElement(ns + "g", new XAttribute("data-cell-id", layer.Id));
            layerGroup.SetAttributeValue("data-drawvista-layer-id", layer.Id);
            layerGroup.SetAttributeValue("data-drawvista-layer-name", layer.Name);
            layerGroup.SetAttributeValue("data-drawvista-layer-visible", layer.Visible ? "true" : "false");
            layerGroup.SetAttributeValue("display", layer.Visible ? null : "none");
            return layerGroup;
        }).ToList();
        group.ReplaceNodes(orderedGroups);
        svg.SetAttributeValue("data-drawvista-layers", "1");
        return svg.ToString();
    }

    private static XElement ReadPage(XElement diagram)
    {
        if (diagram.Element("mxGraphModel") is { } graph)
        {
            return new XElement(graph);
        }
        if (diagram.Value.TrimStart().StartsWith('<'))
        {
            return XElement.Parse(diagram.Value);
        }

        using var compressed = new MemoryStream(Convert.FromBase64String(diagram.Value));
        using var deflate = new DeflateStream(compressed, CompressionMode.Decompress);
        using var reader = new StreamReader(deflate);
        return XElement.Parse(Uri.UnescapeDataString(reader.ReadToEnd()));
    }

    private static XElement GraphRoot(XElement graph) =>
        graph.Element("root") ?? throw new InvalidDataException("Draw.io page has no graph root.");

    private static IReadOnlyList<XElement> LayerElements(XElement graph)
    {
        var root = GraphRoot(graph);
        var rootId = CellId(root.Elements().Single(x => Cell(x).Attribute("parent") == null));
        return root.Elements().Where(x => (string)Cell(x).Attribute("parent") == rootId).ToList();
    }

    private static XElement Cell(XElement element) =>
        element.Name == "mxCell" ? element : element.Element("mxCell")
            ?? throw new InvalidDataException("Draw.io graph contains an invalid cell.");

    private static string CellId(XElement element) =>
        (string)element.Attribute("id") ?? (string)Cell(element).Attribute("id")
            ?? throw new InvalidDataException("Draw.io cell has no identity.");
}

public record DrawIOLayer(string Id, string Name, bool Visible);
