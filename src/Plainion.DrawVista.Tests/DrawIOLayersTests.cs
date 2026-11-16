using System.Globalization;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Data.Sqlite;
using Plainion.DrawVista.Adapters;
using Plainion.DrawVista.IO;
using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.Tests;

[TestFixture]
public class DrawIOLayersTests
{
    private string myFolder;
    private string myExports;
    private string myFixture;
    private string myPngFixture;
    private byte[] mySourceBytes;
    private IReadOnlyCollection<RawDocument> myDocuments;
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";

    [OneTimeSetUp]
    public void SetUp()
    {
        myFolder = Path.Combine(Path.GetTempPath(), $"DrawVista.Layers-{Guid.NewGuid():N}");
        myExports = Path.Combine(myFolder, "exports");
        Directory.CreateDirectory(myExports);
        myFixture = Path.Combine(TestContext.CurrentContext.TestDirectory, "Fixtures", "Layers.drawio");
        myPngFixture = Path.Combine(TestContext.CurrentContext.TestDirectory, "Fixtures", "Layers.drawio.png");
        mySourceBytes = File.ReadAllBytes(myFixture);
        using var input = File.OpenRead(myFixture);
        myDocuments = new DrawingWorkbookFactory(myExports).TryCreate("Layers.drawio").Load(input);
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        if (Directory.Exists(myFolder))
        {
            Directory.Delete(myFolder, true);
        }
    }

    [Test]
    public void ImportsAllPagesWithSourceLayerOrderVisibilityAndOneCommonCoordinateSystem()
    {
        Assert.That(myDocuments.Select(x => x.Name), Is.EqualTo(new[] { "Layers", "Second" }));
        AssertLayers(XElement.Parse(myDocuments.First().Content), [true, true, false]);
        AssertLayers(XElement.Parse(myDocuments.Last().Content), [true, false, true]);
        Assert.That(File.ReadAllBytes(myFixture), Is.EqualTo(mySourceBytes));
        Assert.That(Directory.GetFiles(myExports), Is.Empty);
    }

    [TestCase("Layers.drawio.png", "Layers", true, true, false)]
    [TestCase("Second.drawio.png", "Second", true, false, true)]
    [TestCase("Unknown.DRAWIO.PNG", "Unknown", true, true, false)]
    public void ImportsEmbeddedPngWithExistingNamingAndPageSelection(
        string fileName, string pageName, bool background, bool foreground, bool hidden)
    {
        var original = File.ReadAllBytes(myPngFixture);
        using var input = File.OpenRead(myPngFixture);

        var documents = new DrawingWorkbookFactory(myExports).TryCreate(fileName).Load(input);

        Assert.That(documents, Has.Count.EqualTo(1));
        Assert.That(documents.Single().Name, Is.EqualTo(pageName));
        AssertLayers(XElement.Parse(documents.Single().Content), [background, foreground, hidden]);
        Assert.That(File.ReadAllBytes(myPngFixture), Is.EqualTo(original));
        Assert.That(File.ReadAllBytes(myFixture), Is.EqualTo(mySourceBytes));
        Assert.That(Directory.GetFiles(myExports), Is.Empty);
    }

    [TestCase(false)]
    [TestCase(true)]
    public void ProcessingAndBothStoresPreserveLayersHiddenCaptionsLinksAndLegacyPages(bool sqlite)
    {
        var folder = Path.Combine(myFolder, sqlite ? "sqlite" : "filesystem");
        Directory.CreateDirectory(folder);
        IDocumentStore CreateStore()
        {
            if (!sqlite)
            {
                return new FileSystemDocumentStore(folder);
            }
            var store = new SqliteDocumentStore(new SqliteConnectionStringBuilder
            {
                DataSource = Path.Combine(folder, "store.db"),
                Pooling = false
            }.ToString());
            store.Init();
            return store;
        }

        const string legacy = """<svg xmlns="http://www.w3.org/2000/svg" width="100%" viewBox="0 0 100 100"><rect x="5" y="10" width="50" height="60" /></svg>""";
        var store = new DocumentStoreCachingDecorator(CreateStore());
        store.Save(new ProcessedDocument("Legacy", legacy, []));
        var processor = new SvgProcessor(new SvgCaptionParser(), new SvgHyperlinkFormatter(), store);
        processor.Process(myDocuments);

        var reloaded = new DocumentStoreCachingDecorator(CreateStore());
        var document = reloaded.GetPage("Layers");
        var svg = XElement.Parse(document.Content);
        AssertLayers(svg, [true, true, false]);
        Assert.That(document.Captions, Does.Contain("HiddenOnly").And.Contain("Second").And.Contain("Website"));
        Assert.That(new FullTextSearch(reloaded).Search("HiddenOnly").Single().PageName, Is.EqualTo("Layers"));
        var hidden = LayerGroups(svg).Last();
        var generatedLink = hidden.Descendants()
            .Single(x => x.Name.LocalName == "div" && !x.HasElements && x.Value == "Second");
        Assert.That(generatedLink.Attribute("onclick")?.Value, Is.EqualTo("window.hook.navigate('Second')"));
        Assert.That(hidden.Descendants(Svg + "a")
            .Single(x => x.Attributes().Any(a => a.Name.LocalName == "href"))
            .Attributes().Single(x => x.Name.LocalName == "href").Value,
            Is.EqualTo("https://example.com/layers"));
        Assert.That(document.Content, Is.EqualTo(store.GetPage("Layers").Content));
        var legacySvg = XElement.Parse(reloaded.GetPage("Legacy").Content);
        Assert.That(legacySvg.Attribute("data-drawvista-layers"), Is.Null);
        Assert.That(legacySvg.Element(Svg + "rect").ToString(),
            Is.EqualTo(XElement.Parse(legacy).Element(Svg + "rect").ToString()));
    }

    [Test]
    public void CompressedPagesAndWrappedLayerCellsPreserveTheirMetadata()
    {
        var graph = XElement.Parse("""
            <mxGraphModel><root>
              <mxCell id="root" />
              <mxCell id="base" parent="root" />
              <object id="overlay" label="Named &amp; wrapped">
                <mxCell parent="root" visible="0" />
              </object>
            </root></mxGraphModel>
            """);
        using var compressed = new MemoryStream();
        using (var deflate = new DeflateStream(compressed, CompressionMode.Compress, leaveOpen: true))
        {
            deflate.Write(Encoding.UTF8.GetBytes(Uri.EscapeDataString(graph.ToString())));
        }
        var xml = new XElement("mxfile", new XElement("diagram", new XAttribute("name", "Compressed"),
            Convert.ToBase64String(compressed.ToArray()))).ToString();
        var model = new DrawIOModel(xml);

        Assert.That(model.GetPageNames(), Is.EqualTo(new[] { "Compressed" }));
        Assert.That(model.GetLayers(0), Is.EqualTo(new[]
        {
            new DrawIOLayer("base", "Background", true),
            new DrawIOLayer("overlay", "Named & wrapped", false)
        }));
    }

    [Test]
    public void ImportsCompressedWorkbookThroughTheRealExporter()
    {
        var xml = XElement.Load(myFixture);
        foreach (var diagram in xml.Elements("diagram"))
        {
            using var compressed = new MemoryStream();
            using (var deflate = new DeflateStream(compressed, CompressionMode.Compress, leaveOpen: true))
            {
                deflate.Write(Encoding.UTF8.GetBytes(Uri.EscapeDataString(diagram.Element("mxGraphModel").ToString())));
            }
            diagram.ReplaceNodes(Convert.ToBase64String(compressed.ToArray()));
        }
        using var input = new MemoryStream(Encoding.UTF8.GetBytes(xml.ToString()));

        var documents = new DrawingWorkbookFactory(myExports).TryCreate("Compressed.drawio").Load(input).ToList();

        Assert.That(documents.Select(x => x.Name), Is.EqualTo(new[] { "Layers", "Second" }));
        AssertLayers(XElement.Parse(documents[0].Content), [true, true, false]);
        AssertLayers(XElement.Parse(documents[1].Content), [true, false, true]);
        Assert.That(Directory.GetFiles(myExports), Is.Empty);
    }

    [Test]
    public void DefaultOnlyAndEmptyLayersKeepMetadataWithoutDuplicatingContent()
    {
        foreach (var content in new[]
        {
            """<mxfile><diagram name="WithEmptyLayer"><mxGraphModel><root><mxCell id="0"/><mxCell id="1" parent="0"/><mxCell id="empty" value="Empty" parent="0" visible="0"/><mxCell id="box" value="Default" vertex="1" parent="1"><mxGeometry x="10" y="10" width="80" height="40" as="geometry"/></mxCell></root></mxGraphModel></diagram></mxfile>""",
            """<mxfile><diagram name="Default"><mxGraphModel><root><mxCell id="0"/><mxCell id="1" parent="0"/><mxCell id="box" value="Default" vertex="1" parent="1"><mxGeometry x="10" y="10" width="80" height="40" as="geometry"/></mxCell></root></mxGraphModel></diagram></mxfile>"""
        })
        {
            using var input = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var document = new DrawingWorkbookFactory(myExports).TryCreate("Default.drawio").Load(input).Single();
            var svg = XElement.Parse(document.Content);
            var layers = LayerGroups(svg);
            Assert.That(layers, Has.Count.EqualTo(document.Name == "WithEmptyLayer" ? 2 : 1));
            Assert.That(svg.Descendants(Svg + "rect").Count(), Is.EqualTo(1));
        }
        Assert.That(Directory.GetFiles(myExports), Is.Empty);
    }

    private static List<XElement> LayerGroups(XElement svg) =>
        svg.Descendants(Svg + "g").Where(x => x.Attribute("data-drawvista-layer-id") != null).ToList();

    private static void AssertLayers(XElement svg, bool[] visibility)
    {
        Assert.That(svg.Attribute("data-drawvista-layers")?.Value, Is.EqualTo("1"));
        var layers = LayerGroups(svg);
        Assert.That(layers.Select(x => x.Attribute("data-drawvista-layer-id")?.Value),
            Is.EqualTo(new[] { "1", "foreground", "hidden" }));
        Assert.That(layers.Select(x => x.Attribute("data-drawvista-layer-name")?.Value),
            Is.EqualTo(new[] { "Background", "Foreground", "Hidden" }));
        Assert.That(layers.Select(x => x.Attribute("data-drawvista-layer-visible")?.Value),
            Is.EqualTo(visibility.Select(x => x ? "true" : "false")));
        Assert.That(layers.Select(x => (string)x.Attribute("display") == "none"),
            Is.EqualTo(visibility.Select(x => !x)));

        if (svg.Descendants().Any(x => (string)x.Attribute("data-cell-id") == "outlier"))
        {
            var boxes = new[] { "background-box", "foreground-box", "outlier" }
                .Select(id => svg.Descendants().Single(x => (string)x.Attribute("data-cell-id") == id)
                    .Descendants(Svg + "rect").Single()).ToList();
            Assert.That(boxes.Select(x => double.Parse(x.Attribute("x").Value, CultureInfo.InvariantCulture)),
                Is.EqualTo(new[] { 260d, 320d, 0d }));
            Assert.That(boxes.Select(x => double.Parse(x.Attribute("y").Value, CultureInfo.InvariantCulture)),
                Is.EqualTo(new[] { 120d, 160d, 0d }));
            Assert.That(svg.Attribute("viewBox")?.Value, Is.EqualTo("-0.5 -0.5 602 382"));
            Assert.That(svg.Descendants(Svg + "rect").Count(), Is.EqualTo(5));
            Assert.That(svg.Descendants(Svg + "linearGradient"), Is.Not.Empty);
        }

        var ids = svg.DescendantsAndSelf().Attributes("id").Select(x => x.Value).ToList();
        Assert.That(ids.Distinct().Count(), Is.EqualTo(ids.Count));
        foreach (var reference in svg.DescendantsAndSelf().Attributes()
            .SelectMany(x => Regex.Matches(x.Value, @"url\(#([^)]+)\)").Select(match => match.Groups[1].Value)))
        {
            Assert.That(ids, Does.Contain(reference));
        }
    }
}
