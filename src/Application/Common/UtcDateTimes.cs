namespace Application.Common;

public static class UtcDateTimes
{
    /// <summary>
    /// Normalizes a client-supplied timestamp to UTC. A value without an offset is taken
    /// to already be UTC, which also keeps Npgsql happy: writing a <c>DateTime</c> whose
    /// Kind is Unspecified into a <c>timestamptz</c> column throws.
    /// </summary>
    public static DateTime Normalize(
        DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }

    public static DateTime? Normalize(
        DateTime? value)
    {
        return value is null
            ? null
            : Normalize(value.Value);
    }
}
