using System.Xml.Linq;
using Microsoft.Data.Sqlite;
using Plainion.DrawVista.Adapters;
using Plainion.DrawVista.IO;
using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.Tests;

[TestFixture]
public class StartPageTests
{
    private string myDataFolder;
    private IDocumentStore myStore;
    private SvgProcessor myProcessor;
    private StartPage myStartPage;

    [SetUp]
    public void SetUp()
    {
        var options = InstanceOptions.Parse(["-name", $"DrawVista.Tests-{Guid.NewGuid():N}"]);
        myDataFolder = options.DataFolder;
        Directory.CreateDirectory(myDataFolder);

        var connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = Path.Combine(myDataFolder, "store.db"),
            Pooling = false
        }.ToString();
        var sqliteStore = new SqliteDocumentStore(connectionString);
        sqliteStore.Init();

        myStore = new DocumentStoreCachingDecorator(sqliteStore);
        var parser = new SvgCaptionParser();
        myProcessor = new SvgProcessor(parser, new SvgHyperlinkFormatter(), myStore);
        myStartPage = new StartPage(myStore, parser, options);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(myDataFolder))
        {
            Directory.Delete(myDataFolder, true);
        }
    }

    [Test]
    public void UploadAddsPagesAndLinksToExistingIndex()
    {
        myProcessor.Process([CreateDocument("Page-1", "Page-2")]);
        Assert.That(GetGraphTitles("node"), Is.EquivalentTo(new[] { "Page-1" }));

        myProcessor.Process([CreateDocument("Page-2")]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(GetGraphTitles("node"), Is.EquivalentTo(new[] { "Page-1", "Page-2" }));
            Assert.That(GetGraphTitles("edge"), Is.EquivalentTo(new[] { "Page-1->Page-2" }));
            var link = XElement.Parse(myStartPage.Svg)
                .Descendants()
                .Single(x => x.Name.LocalName == "text" && x.Value == "Page-2");
            Assert.That(link.Attribute("onclick")?.Value, Is.EqualTo("window.hook.navigate('Page-2')"));
        }
    }

    [Test]
    public void UploadReplacesLinksInExistingIndex()
    {
        myProcessor.Process([CreateDocument("Page-1", "Page-2"), CreateDocument("Page-2")]);
        Assert.That(GetGraphTitles("edge"), Is.EquivalentTo(new[] { "Page-1->Page-2" }));

        myProcessor.Process([CreateDocument("Page-1", "Updated")]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(GetGraphTitles("node"), Is.EquivalentTo(new[] { "Page-1", "Page-2" }));
            Assert.That(GetGraphTitles("edge"), Is.Empty);
        }
    }

    [Test]
    public void ClearingDocumentsRecreatesEmptyIndex()
    {
        myProcessor.Process([CreateDocument("Page-1")]);
        Assert.That(GetGraphTitles("node"), Is.EquivalentTo(new[] { "Page-1" }));

        myStore.Clear();

        Assert.That(GetGraphTitles("node"), Is.Empty);
    }

    private IReadOnlyCollection<string> GetGraphTitles(string elementClass)
    {
        XNamespace ns = "http://www.w3.org/2000/svg";
        return XElement.Parse(myStartPage.Svg)
            .Descendants(ns + "g")
            .Where(x => x.Attribute("class")?.Value == elementClass)
            .Select(x => x.Element(ns + "title").Value)
            .ToList();
    }

    private static RawDocument CreateDocument(string name, params string[] captions)
    {
        var document = DocumentBuilder.Create(name, captions);
        var content = XElement.Parse(document.Content);
        content.SetAttributeValue("width", "100%");
        return new RawDocument(name, content.ToString());
    }
}
