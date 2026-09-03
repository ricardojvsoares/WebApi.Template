using System.Reflection;

namespace WebApi;

#pragma warning disable CA1515 // Consider making public types internal
public static class AssemblyReference
#pragma warning restore CA1515 // Consider making public types internal
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
