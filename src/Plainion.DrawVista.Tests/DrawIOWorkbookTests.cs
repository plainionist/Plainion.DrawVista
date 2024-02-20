using Plainion.DrawVista.IO;

namespace Plainion.DrawVista.Tests;

[TestFixture]
public class DrawIOWorkbookTests
{
    private readonly string myRootFolder = Path.Combine(Path.GetTempPath(), "DrawIOWorkbookTests");

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
    }
}
