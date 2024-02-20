using Plainion.DrawVista.IO;
using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.Tests;

[TestFixture]
public class DrawIOPngWorkbookTests
{
    private readonly string myRootFolder = Path.Combine(Path.GetTempPath(), "DrawIOPngWorkbookTests");

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
        var documents = new List<SvgDocument>();

        using (var stream = File.OpenRead("../../../../../samples/Simple/Index.drawio.png"))
        {
            var workbook = new DrawIOPngWorkbook(myRootFolder, "Index.drawio.png");
            documents.AddRange(workbook.Load(stream));
        }
        using (var stream = File.OpenRead("../../../../../samples/Simple/Parser.drawio.png"))
        {
            var workbook = new DrawIOPngWorkbook(myRootFolder, "Parser.drawio.png");
            documents.AddRange(workbook.Load(stream));
        }

        Assert.That(documents.Select(x => x.Name), Is.EquivalentTo(new string[] { "Index", "Parser" }));
    }
}
