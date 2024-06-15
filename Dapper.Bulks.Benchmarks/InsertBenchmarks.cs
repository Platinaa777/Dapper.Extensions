using System.Data;
using BenchmarkDotNet.Attributes;
using Npgsql;

namespace Dapper.Bulks.Benchmarks;

public class InsertBenchmarks
{
    private readonly IDbConnection _connection;
    private readonly DapperContext _context;
    private readonly IList<Document> _entities;

    public InsertBenchmarks()
    {
        _connection = new NpgsqlConnection("User ID=denis;password=denis123;port=5434;host=localhost;database=orm_db");
        _context = new DapperContext(_connection);
        _entities = GenerateEntities(1000);
        _context.Entity<Document>("test_table");
    }

    [Benchmark]
    public async Task<int> MyBulkInsert()
    {
        return await _context.BulkInsertAsync(new InsertColumn("id, content"), _entities);
    }

    [Benchmark(Baseline = true)]
    public async Task<int> DapperExecute()
    {
        var sql = "INSERT INTO test_table (id, content) VALUES (@Id, @Content)";
        return await _connection.ExecuteAsync(sql, _entities);
    }

    private IList<Document> GenerateEntities(int count)
    {
        var entities = new List<Document>();
        for (int i = 0; i < count; i++)
            entities.Add(new Document { Id = i, Content = $"Content - {i}"});
        return entities;
    }

    class Document
    {
        public int Id { get; set; }
        public string Content { get; set; }
    }
}