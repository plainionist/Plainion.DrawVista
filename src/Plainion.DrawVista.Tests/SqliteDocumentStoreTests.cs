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
        connection.CloseAsync().ContinueWith((Task task) => {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            File.Delete("test.db");
        });
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
        Assert.That(1, Is.EqualTo(documents.Count));

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
}
