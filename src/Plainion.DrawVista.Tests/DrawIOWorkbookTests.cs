using Plainion.DrawVista.IO;

namespace Plainion.DrawVista.Tests;

[TestFixture]
public class DrawIOWorkbookTests
{
    [Test]
    public void ExtractAllPages()
    {
        var rootFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(rootFolder);
        var workbook = new DrawIOWorkbook(rootFolder, "sample.drawio");

        using var stream = File.OpenRead(@"../../../../../samples/sample.drawio");
        var documents = workbook.Load(stream);

        Assert.That(documents.Select(x => x.Name), Is.EquivalentTo(
            new string[] { "index", "orchestrator", "TestFailureAnalyzer", "ReadinessChecker" }));
    }
}
