using System.Data;
using System.Reflection;
using System.Text;

namespace Dapper.Bulks;

public static class DapperBulkInsert
{
    public static async Task<IDbConnection> BulkInsertAsync<T>(this IDbConnection connection,
        Table table,
        InsertColumn insertColumn,
        IList<T> collection)
    {
        if (collection.Count == 0)
            return connection;

        var props = typeof(T)
            .GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            .Where(x => insertColumn.Contains(x.Name))
            .ToArray();
        
        var insertQuery = new StringBuilder();
        insertQuery.Append($"INSERT INTO {table} ({insertColumn}) VALUES ");
    
        var totalInsertBuilder = new StringBuilder();
        var parameters = new DynamicParameters();

        for (var i = 0; i < collection.Count; i++)
        {
            var rowBuilder = new StringBuilder();
            foreach (var prop in props)
            {
                var paramName = $"@{prop.Name}_{i}";
                rowBuilder.Append(paramName);
                rowBuilder.Append(", ");
                var propValue = prop.GetValue(collection.ElementAt(i));
                 
                parameters.Add(paramName, propValue);
            }

            rowBuilder.Remove(rowBuilder.Length - 2, 2);
            totalInsertBuilder.Append('(');
            totalInsertBuilder.Append(rowBuilder);
            totalInsertBuilder.Append("), ");
        }

        totalInsertBuilder.Remove(totalInsertBuilder.Length - 2, 2);
        insertQuery.Append(totalInsertBuilder);

        var sql = insertQuery.ToString(); 
    
        await connection.ExecuteAsync(sql, parameters);

        return connection;
    }
}