using System.Reflection;
using Microsoft.AspNetCore.Routing;

namespace SharpEndpoint;

public static class Extensions
{
    /// <summary>
    /// <c>MapSharpEndpointFragmentsFromAssembly</c> will map all the <c>MapSharpEndpointFragment</c> from the given assembly using reflection.
    /// </summary>
    public static IEndpointRouteBuilder MapSharpEndpointFragmentsFromAssembly(this IEndpointRouteBuilder endpoints,
        Assembly assembly)
    {
        var types = assembly.GetTypes().Where(t =>
            t is { IsAbstract: false, IsInterface: false } && t.IsAssignableTo(typeof(SharpEndpointFragment)));

        foreach (var type in types)
        {
            var constructors = type.GetConstructors();
            if (constructors.Length > 1 || (constructors.Length == 1 && constructors[0].GetParameters().Length > 0))
            {
                throw new InvalidOperationException(
                    $"Only a non-parameterized constructor is allowed in Type {type.FullName}");
            }

            var instance = Activator.CreateInstance(type) as SharpEndpointFragment;

            var method = typeof(SharpEndpointFragment)
                .GetMethod("MapSharpEndpointFragment__DO_NOT_OVERRIDE",
                    BindingFlags.Instance | BindingFlags.NonPublic);

            method?.Invoke(instance, [endpoints]);
        }

        return endpoints;
    }
}