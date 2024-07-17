namespace SharpEndpoint.HttpApiExample.Utils;

public record PagedData<TData>(TData Data, int TotalDataCount);