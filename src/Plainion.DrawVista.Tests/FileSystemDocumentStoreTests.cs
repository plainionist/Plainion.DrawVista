using Plainion.DrawVista.IO;
using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.Tests;

[TestFixture]
public class FileSystemDocumentStoreTests
{
    private readonly string myRootFolder = Path.Combine(Path.GetTempPath(), "DrawVista.Store");

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
    public void StoreAndLoad()
    {
        var store = new FileSystemDocumentStore(myRootFolder);

        store.Save(new ProcessedDocument("Page-1", "Some dummy content", ["caption-1", "caption-2"]));
        var document = store.GetPage("Page-1");

        Assert.That(document.Captions, Is.EquivalentTo(new[] { "caption-1", "caption-2" }));
    }
}
