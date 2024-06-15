namespace Dapper.Bulks;

public class InsertColumn
{
    private readonly string[] _columns;

    public InsertColumn(params string[] columns)
    {
        _columns = columns.Select(x => x.ToLower()).ToArray();
    }

    public override string ToString()
    {
        return string.Join(", ", _columns);
    }

    public bool Contains(string column)
    {
        return _columns.Contains(column.ToLower());
    }

    public static implicit operator InsertColumn(string[] columns)
    {
        return new InsertColumn(columns);
    }
}