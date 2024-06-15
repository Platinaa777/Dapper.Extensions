using System.Collections.Concurrent;

namespace Dapper.Bulks;

public static class DapperContextExtensions
{
    private static readonly ConcurrentDictionary<Type, string> _mappings = new();

    public static void Entity<T>(this DapperContext _, string tableName)
    {
        _mappings.TryAdd(typeof(T), tableName);
    }

    public static bool TryGetMapping(this DapperContext ctx, Type entityType, out string columnName)
    {
        if (!_mappings.TryGetValue(entityType, out var table))
        {
            columnName = default!;
            return false;
        }

        columnName = table;
        return true;
    }
}