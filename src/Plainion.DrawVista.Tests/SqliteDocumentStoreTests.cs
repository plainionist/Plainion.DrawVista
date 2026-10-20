using Dapper;
using Microsoft.Data.Sqlite;
using Plainion.DrawVista.IO;
using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.Tests;

[TestFixture]
public class SqliteDocumentStoreTests
{
    private SqliteConnection connection;

    [SetUp]
    public void SetUp()
    {
        connection = new SqliteConnection("Data Source=test.db");

        var sql = """
            CREATE TABLE IF NOT EXISTS
            Pages (
                Name TEXT NOT NULL PRIMARY KEY,
                Content TEXT,
                Captions TEXT
            )
        """;

        connection.Execute(sql);
    }

    [TearDown]
    public void TearDown()
    {
        connection.Dispose();
        SqliteConnection.ClearAllPools();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        File.Delete("test.db");
    }

    [Test]
    public void StoreAndLoad()
    {
        var store = new SqliteDocumentStore("Data Source=test.db");

        store.Save(new ProcessedDocument("Page-1", "Some dummy content", ["caption-1", "caption-2"]));
        var document = store.GetPage("Page-1");

        Assert.That(document.Captions, Is.EquivalentTo(new[] { "caption-1", "caption-2" }));
    }

    [Test]
    public void ClearStore()
    {
        //Arrange
        var store = new SqliteDocumentStore("Data Source=test.db");

        store.Save(new ProcessedDocument("Page-1", "Some dummy content", ["caption-1", "caption-2"]));
        var documents = store.GetPageNames();
        Assert.That(documents.Count, Is.EqualTo(1));

        //Act
        store.Clear();
        documents = store.GetPageNames();

        //Assert
        Assert.That(documents, Is.Empty);
    }

    [Test]
    public void StoreNewPage_Always_DocumentsChangedEventRaised()
    {
        //Arrange
        var eventRaised = false;

        var store = new SqliteDocumentStore("Data Source=test.db");
        store.DocumentsChanged += (sender, eventArgs) => {
            eventRaised = true;
        };

        //Act
        store.Save(new ProcessedDocument("Page-1", "Some dummy content", ["caption-1", "caption-2"]));

        //Assert
        Assert.That(eventRaised, Is.True);
    }

    [Test]
    public void DeleteRemovesOnlyNamedPageAndRaisesChangeOnce()
    {
        var store = new SqliteDocumentStore("Data Source=test.db");
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
            Assert.That(store.GetPage("Page-1"), Is.Null);
            Assert.That(store.GetPageNames(), Is.EquivalentTo(new[] { "Page-2" }));
            Assert.That(changes, Is.EqualTo(1));
        }
    }

    [Test]
    public void DeleteUnknownPageLeavesStoreUnchanged()
    {
        var store = new SqliteDocumentStore("Data Source=test.db");
        store.Save(DocumentBuilder.Create("Page-1"));
        var changes = 0;
        store.DocumentsChanged += (_, _) => changes++;

        var deleted = store.Delete("Unknown");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(deleted, Is.False);
            Assert.That(store.GetPageNames(), Is.EquivalentTo(new[] { "Page-1" }));
            Assert.That(changes, Is.Zero);
        }
    }
}
