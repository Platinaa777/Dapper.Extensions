namespace Dapper.Bulks;

public class MappingException(Type type)
    : Exception($"Mapping for type {type} not registered");