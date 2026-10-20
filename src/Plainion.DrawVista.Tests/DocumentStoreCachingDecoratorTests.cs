using Plainion.DrawVista.Adapters;

namespace Plainion.DrawVista.Tests;

[TestFixture]
public class DocumentStoreCachingDecoratorTests
{
    [Test]
    public void DeleteEvictsPageFromStoreAndCacheAndRaisesChangeOnce()
    {
        var innerStore = new FakeDocumentStore();
        innerStore.Save(DocumentBuilder.Create("Page-1"));
        innerStore.Save(DocumentBuilder.Create("Page-2"));
        var store = new DocumentStoreCachingDecorator(innerStore);
        var changes = 0;
        store.DocumentsChanged += (_, _) => changes++;

        var deleted = store.Delete("Page-1");
        var deletedAgain = store.Delete("Page-1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(deleted, Is.True);
            Assert.That(deletedAgain, Is.False);
            Assert.That(store.GetPageNames(), Is.EquivalentTo(new[] { "Page-2" }));
            Assert.That(innerStore.GetPageNames(), Is.EquivalentTo(new[] { "Page-2" }));
            Assert.That(() => store.GetPage("Page-1"), Throws.TypeOf<KeyNotFoundException>());
            Assert.That(changes, Is.EqualTo(1));
        }
    }

    [Test]
    public void DeleteUnknownPageDoesNotRaiseChange()
    {
        var store = new DocumentStoreCachingDecorator(new FakeDocumentStore());
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
