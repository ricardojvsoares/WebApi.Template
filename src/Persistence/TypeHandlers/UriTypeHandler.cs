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
        parameter.Value = value is null
            ? DBNull.Value
            : value.AbsoluteUri;
    }

    public override Uri Parse(
        object value)
    {
        return new Uri((string)value, UriKind.Absolute);
    }
}
