
## Реализовано только bulk insert

Видно, что библиотека позволяет повысить скорость вставки коллекции элементов, но при этом происходит больше аллокаций при использовании библиотеки

Тест на 1000 элементов за 1 раз


| Method        | Mean         | Error      | StdDev      | Ratio | Gen0     | Gen1     | Gen2    | Allocated | Alloc Ratio |
|-------------- |-------------:|-----------:|------------:|------:|---------:|---------:|--------:|----------:|------------:|
| MyBulkInsert  |     5.581 ms |  0.1049 ms |   0.1078 ms | 0.004 | 281.2500 | 179.6875 | 62.5000 |   1.54 MB |        1.31 |
| DapperExecute | 1,549.074 ms | 57.3793 ms | 167.3780 ms | 1.000 |        - |        - |       - |   1.18 MB |        1.00 |

Пример использования:

```csharp
var columns = new InsertColumn(["id", "content"]);
var docs = new List<Document>();
await connection.BulkInsertAsync(tableName, columns, docs);
```