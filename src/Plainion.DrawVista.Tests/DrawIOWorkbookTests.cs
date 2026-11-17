using Plainion.DrawVista.IO;

namespace Plainion.DrawVista.Tests;

[TestFixture]
public class DrawIOWorkbookTests
{
    private readonly string myRootFolder = Path.Combine(Path.GetTempPath(), "DrawIOWorkbookTests with spaces");

    [SetUp]
    public void SetUp()
    {
        Directory.CreateDirectory(myRootFolder);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(myRootFolder))
        {
            Directory.Delete(myRootFolder, true);
        }
    }

    [Test]
    public void ExtractAllPages()
    {
        var workbook = new DrawIOWorkbook(myRootFolder, "sample.drawio");

        using var stream = File.OpenRead("../../../../../samples/sample.drawio");
        var documents = workbook.Load(stream);

        Assert.That(documents.Select(x => x.Name), Is.EquivalentTo(
            new string[] { "index", "orchestrator", "TestFailureAnalyzer", "ReadinessChecker" }));
        Assert.That(Directory.GetFiles(myRootFolder, "*.drawio"), Is.Empty);
    }

    [TestCase("25.0.2", 0, "0")]
    [TestCase("25.0.2", 1, "1")]
    [TestCase("27.0.1", 0, "0")]
    [TestCase("27.0.2", 0, "1")]
    [TestCase("27.0.2", 1, "2")]
    [TestCase("32.0.0.0", 0, "1")]
    [TestCase("32.0.0.0", 1, "2")]
    public void ExportRequestsTheCorrectPageForDrawIoVersion(string version, int pageIndex, string expectedPage)
    {
        var startInfo = DrawIOApp.CreateExportStartInfo(
            "model.drawio", "page.svg", pageIndex, Version.Parse(version));

        var pageOption = startInfo.ArgumentList.IndexOf("-p");
        Assert.That(pageOption, Is.GreaterThanOrEqualTo(0));
        Assert.That(startInfo.ArgumentList[pageOption + 1], Is.EqualTo(expectedPage));
    }
}
