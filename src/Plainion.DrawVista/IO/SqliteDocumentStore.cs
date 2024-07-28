using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using Plainion.DrawVista.UseCases;

namespace Plainion.DrawVista.IO;

public class SqliteDocumentStore : IDocumentStore
{
    private readonly string myConnectionString;

    public event EventHandler DocumentsChanged;

    public SqliteDocumentStore(string appHome)
    {
        myConnectionString = $"Data Source={appHome}\\store.db";

        SqlMapper.AddTypeHandler(new StringCollectionHandler());
    }

    public void Init()
    {
        using var connection = new SqliteConnection(myConnectionString);

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

    public IReadOnlyCollection<string> GetPageNames()
    {
        using var connection = new SqliteConnection(myConnectionString);

        return connection.Query<string>("SELECT Name FROM Pages").ToList();
    }

    public ProcessedDocument GetPage(string pageName)
    {
        using var connection = new SqliteConnection(myConnectionString);

        var sql = "SELECT * FROM Pages WHERE Name = @Name";

        return connection.QuerySingleOrDefault<ProcessedDocument>(sql, new { Name = pageName });
    }

    public void Save(ProcessedDocument document)
    {
        using var connection = new SqliteConnection(myConnectionString);

        var sql = """
            INSERT INTO Pages (Name, Content, Captions)
            VALUES (@Name, @Content, @Captions)
            ON CONFLICT(Name) DO UPDATE SET
            Content = excluded.Content,
            Captions = excluded.Captions
        """;

        int affectedRows = connection.Execute(sql, document);

        if (affectedRows > 0)
        {
            DocumentsChanged?.Invoke(this, null);
        }
    }

    public void Clear()
    {
        using var connection = new SqliteConnection(myConnectionString);

        connection.Execute("DELETE FROM Pages");
    }

    class StringCollectionHandler : SqlMapper.TypeHandler<IReadOnlyCollection<string>>
    {
        public override IReadOnlyCollection<string> Parse(object value)
        {
            return JsonConvert.DeserializeObject<IReadOnlyCollection<string>>((string)value);
        }

        public override void SetValue(IDbDataParameter parameter, IReadOnlyCollection<string> value)
        {
            parameter.Value = JsonConvert.SerializeObject(value);
        }

    }
}
