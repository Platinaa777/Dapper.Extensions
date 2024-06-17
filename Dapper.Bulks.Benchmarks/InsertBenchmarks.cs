using System.Data;
using BenchmarkDotNet.Attributes;
using Npgsql;

namespace Dapper.Bulks.Benchmarks;

[MemoryDiagnoser]
public class InsertBenchmarks
{
    private readonly IDbConnection _connection;
    private readonly List<Document> _entities;
    private readonly Table _table;

    public InsertBenchmarks()
    {
        _connection = new NpgsqlConnection("User ID=denis;password=denis123;port=5434;host=localhost;database=orm_db");
        _entities = GenerateEntities(1000);
        _table = new Table("test_table");
    }

    [Benchmark]
    public async Task<IDbConnection> MyBulkInsert()
    {
        return await _connection.BulkInsertAsync(_table, new InsertColumn(["id", "content"]), _entities);
    }

    [Benchmark(Baseline = true)]
    public async Task<int> DapperExecute()
    {
        var sql = "INSERT INTO test_table (id, content) VALUES (@Id, @Content)";
        return await _connection.ExecuteAsync(sql, _entities);
    }

    private List<Document> GenerateEntities(int count)
    {
        var entities = new List<Document>();
        for (int i = 0; i < count; i++)
            entities.Add(new Document(i, $"Content - {i}"));
        return entities;
    }

    record Document(int Id, string Content);
}