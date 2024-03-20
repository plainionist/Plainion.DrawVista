using Plainion.DrawVista.Adapters;
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

        var search = new FullTextSearch(store, new SvgCaptionParser());
        var results = search.Search("UserService");

        var expected = new[] { new SearchResult("Page-1", ["UserService"]) };
        Assert.That(results.ToJson(), Is.EqualTo(expected.ToJson()));
    }

    [Test]
    public void MultiplePagesMatchSubString()
    {
        var store = new FakeDocumentStore();
        store.Save(DocumentBuilder.Create("Page-1", "UserService", "User", "Database"));
        store.Save(DocumentBuilder.Create("Page-2", "ReservationService", "Reservation", "Database"));

        var search = new FullTextSearch(store, new SvgCaptionParser());
        var results = search.Search("base");

        var expected = new[] {
            new SearchResult("Page-1", ["Database"]),
            new SearchResult("Page-2", ["Database"]),
        };
        Assert.That(results.ToJson(), Is.EqualTo(expected.ToJson()));
    }

    [Test]
    public void IgnoreCase()
    {
        var store = new FakeDocumentStore();
        store.Save(DocumentBuilder.Create("Page-1", "UserService", "User", "Database"));
        store.Save(DocumentBuilder.Create("Page-2", "ReservationService", "Reservation", "Database"));

        var search = new FullTextSearch(store, new SvgCaptionParser());
        var results = search.Search("reserv");

        var expected = new[] { new SearchResult("Page-2", ["ReservationService", "Reservation"]) };
        Assert.That(results.ToJson(), Is.EqualTo(expected.ToJson()));
    }

    [Test]
    public void RemoveDuplicateCaptions()
    {
        var store = new FakeDocumentStore();
        store.Save(DocumentBuilder.Create("Page-1", "UserService", "RegistrationService", "userservice"));

        var search = new FullTextSearch(store, new SvgCaptionParser());
        var results = search.Search("user");

        var expected = new[] { new SearchResult("Page-1", ["UserService"]) };
        Assert.That(results.ToJson(), Is.EqualTo(expected.ToJson()));
    }

    [Test]
    public void NoMatch()
    {
        var store = new FakeDocumentStore();
        store.Save(DocumentBuilder.Create("Page-1", "UserService", "User", "Database"));
        store.Save(DocumentBuilder.Create("Page-2", "ReservationService", "Reservation", "Database"));

        var search = new FullTextSearch(store, new SvgCaptionParser());
        var results = search.Search("EventBroker");

        Assert.That(results, Is.Empty);
    }
}
