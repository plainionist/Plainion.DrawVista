using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.Tests;

[TestFixture]
public class FullTextSearchTests
{
    [Test]
    public void SinglePageMatchesFullWord()
    {
        var store = new FakeDocumentStore();
        store.Save(DocumentBuilder.Create("Page-1", "UserService", "User", "Database"));
        store.Save(DocumentBuilder.Create("Page-2", "ReservationService", "Reservation", "Database"));

        var search = new FullTextSearch(store);
        var results = search.Search("UserService");

        Assert.That(results, Is.EquivalentTo(new[] { "Page-1" }));
    }

    [Test]
    public void MultiplePagesMatchSubString()
    {
        var store = new FakeDocumentStore();
        store.Save(DocumentBuilder.Create("Page-1", "UserService", "User", "Database"));
        store.Save(DocumentBuilder.Create("Page-2", "ReservationService", "Reservation", "Database"));

        var search = new FullTextSearch(store);
        var results = search.Search("base");

        Assert.That(results, Is.EquivalentTo(new[] { "Page-1", "Page-2" }));
    }

    [Test]
    public void IgnoreCase()
    {
        var store = new FakeDocumentStore();
        store.Save(DocumentBuilder.Create("Page-1", "UserService", "User", "Database"));
        store.Save(DocumentBuilder.Create("Page-2", "ReservationService", "Reservation", "Database"));

        var search = new FullTextSearch(store);
        var results = search.Search("reserv");

        Assert.That(results, Is.EquivalentTo(new[] { "Page-2" }));
    }

    [Test]
    public void NoMatch()
    {
        var store = new FakeDocumentStore();
        store.Save(DocumentBuilder.Create("Page-1", "UserService", "User", "Database"));
        store.Save(DocumentBuilder.Create("Page-2", "ReservationService", "Reservation", "Database"));

        var search = new FullTextSearch(store);
        var results = search.Search("EventBroker");

        Assert.That(results, Is.Empty);
    }
}
