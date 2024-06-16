using System.Collections.Concurrent;
using System.Data;
using System.Reflection;
using System.Text;

namespace Dapper.Bulks;

public class DapperContext(IDbConnection connection) : IDisposable
{
    public async Task<int> BulkInsertAsync<T>(InsertColumn insertColumn, IList<T> collection)
    {
        if (!this.TryGetMapping(typeof(T), out var table))
            throw new MappingException(typeof(T));
         
        var list = collection.ToList();
        if (list.Count == 0)
            return 0;

        var props = typeof(T)
            .GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            .Where(x => insertColumn.Contains(x.Name))
            .ToArray();
        
        var insertQuery = new StringBuilder();
        insertQuery.Append($"INSERT INTO {table} ({insertColumn}) VALUES ");
    
        var valuesList = new List<string>();
        var parameters = new DynamicParameters();

        for (int i = 0; i < list.Count; i++)
        {
            var valueParams = new List<string>();
            foreach (var prop in props)
            {
                var paramName = $"@{prop.Name}_{i}";
                valueParams.Add(paramName);
                var propValue = prop.GetValue(list.ElementAt(i));
                 
                parameters.Add(paramName, propValue);
            }
            valuesList.Add($"({string.Join(", ", valueParams)})");
        }

        insertQuery.Append(string.Join(", ", valuesList));

        var sql = insertQuery.ToString(); 
    
        await connection.ExecuteAsync(sql, parameters);

        return list.Count;
    }

    public void Dispose()
    {
        connection.Dispose();
    }
}