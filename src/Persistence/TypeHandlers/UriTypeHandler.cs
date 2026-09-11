using System.Data;
using Dapper;

namespace Persistence.TypeHandlers;

internal sealed class UriTypeHandler
    : SqlMapper.TypeHandler<Uri>
{
    public override void SetValue(
        IDbDataParameter parameter,
        Uri? value)
    {
        parameter.DbType = DbType.String;
        parameter.Value = value is null
            ? DBNull.Value
            : value.AbsoluteUri;
    }

    public override Uri Parse(
        object value)
    {
        return value switch
        {
            Uri uri => uri,
            string text when !string.IsNullOrWhiteSpace(text) => new Uri(text, UriKind.Absolute),
            _ => throw new DataException($"Cannot convert {value.GetType().FullName} to {nameof(Uri)}.")
        };
    }
}
