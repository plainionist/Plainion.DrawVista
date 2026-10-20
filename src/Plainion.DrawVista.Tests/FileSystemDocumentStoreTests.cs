using Plainion.DrawVista.IO;
using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.Tests;

[TestFixture]
public class FileSystemDocumentStoreTests
{
    private readonly string myRootFolder = Path.Combine(Path.GetTempPath(), $"DrawVista.Store-{Guid.NewGuid():N}");

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

    [Test]
    public void DeleteRemovesContentAndMetadataAndRaisesChangeOnce()
    {
        var store = new FileSystemDocumentStore(myRootFolder);
        store.Save(DocumentBuilder.Create("Page-1"));
        store.Save(DocumentBuilder.Create("Page-2"));
        var changes = 0;
        store.DocumentsChanged += (_, _) => changes++;

        var deleted = store.Delete("Page-1");
        var deletedAgain = store.Delete("Page-1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(deleted, Is.True);
            Assert.That(deletedAgain, Is.False);
            Assert.That(store.GetPageNames(), Is.EquivalentTo(new[] { "Page-2" }));
            Assert.That(Directory.GetFiles(Path.Combine(myRootFolder, "store")).Select(Path.GetFileName),
                Is.EquivalentTo(new[] { "Page-2.svg", "Page-2.svg.meta" }));
            Assert.That(changes, Is.EqualTo(1));
        }
    }

    [Test]
    public void DeleteUnknownPageDoesNotRaiseChange()
    {
        var store = new FileSystemDocumentStore(myRootFolder);
        var changes = 0;
        store.DocumentsChanged += (_, _) => changes++;

        var deleted = store.Delete("Unknown");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(deleted, Is.False);
            Assert.That(changes, Is.Zero);
        }
    }
}
