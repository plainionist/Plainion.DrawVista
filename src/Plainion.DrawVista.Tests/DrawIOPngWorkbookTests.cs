using Plainion.DrawVista.IO;
using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.Tests;

[TestFixture]
public class DrawIOPngWorkbookTests
{
    [Test]
    public void ExtractAllPages()
    {
        var rootFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(rootFolder);

        var documents = new List<SvgDocument>();
        using (var stream = File.OpenRead(@"../../../../../samples/Simple/Index.drawio.png"))
        {
            var workbook = new DrawIOPngWorkbook(rootFolder, "Index.drawio.png");
            documents.AddRange(workbook.Load(stream));
        }
        using (var stream = File.OpenRead(@"../../../../../samples/Simple/Parser.drawio.png"))
        {
            var workbook = new DrawIOPngWorkbook(rootFolder, "Parser.drawio.png");
            documents.AddRange(workbook.Load(stream));
        }

        Assert.That(documents.Select(x => x.Name), Is.EquivalentTo(new string[] { "Index", "Parser" }));
    }
}
