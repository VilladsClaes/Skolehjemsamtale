using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Skolehjemsamtale.Infrastructure.Persistence;

namespace Skolehjemsamtale.Tests;

/// <summary>Hjælper der giver en isoleret SQLite-database pr. test.</summary>
public sealed class TestDb : IDisposable
{
    private readonly SqliteConnection _connection;

    public SkoleDbContext Context { get; }

    public TestDb()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder<SkoleDbContext>()
            .UseSqlite(_connection)
            .Options;
        Context = new SkoleDbContext(options);
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}
