using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using UrreaHub.Application.TI;

namespace UrreaHub.Infrastructure.Services;

public static class ApiEndpointDiscovery
{
    public static IReadOnlyList<ApiEndpointDto> DiscoverFromAssembly(Assembly assembly, bool v1Only = false)
    {
        var endpoints = assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Controller", StringComparison.Ordinal))
            .SelectMany(DiscoverFromController)
            .Where(e => !v1Only || e.Route.StartsWith("/api/v1/", StringComparison.OrdinalIgnoreCase))
            .GroupBy(e => $"{e.Method}:{e.Route}")
            .Select(g => g.First())
            .OrderBy(e => e.Route)
            .ThenBy(e => e.Method)
            .ToList();

        return endpoints;
    }

    private static IEnumerable<ApiEndpointDto> DiscoverFromController(Type controllerType)
    {
        var routePrefixes = controllerType
            .GetCustomAttributes<RouteAttribute>(inherit: true)
            .Select(a => NormalizePrefix(a.Template, controllerType))
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Distinct()
            .ToList();

        if (routePrefixes.Count == 0)
            routePrefixes.Add($"/api/{controllerType.Name.Replace("Controller", "", StringComparison.Ordinal).ToLowerInvariant()}");

        var controllerAuth = GetAuthPolicies(controllerType);

        foreach (var method in controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
        {
            var httpMethod = GetHttpMethod(method);
            if (httpMethod is null) continue;

            var methodRoute = GetMethodRouteTemplate(method);
            var methodAuth = GetAuthPolicies(method);
            var auth = methodAuth.Count > 0 ? methodAuth : controllerAuth;

            if (method.GetCustomAttributes<AllowAnonymousAttribute>(inherit: true).Any())
                auth = new[] { "AllowAnonymous" };

            foreach (var prefix in routePrefixes)
            {
                var fullRoute = CombineRoutes(prefix, methodRoute);
                yield return new ApiEndpointDto(httpMethod, fullRoute, controllerType.Name, method.Name, auth);
            }
        }
    }

    private static string NormalizePrefix(string? template, Type controllerType)
    {
        if (string.IsNullOrWhiteSpace(template))
            return "/api";

        var normalized = template
            .Replace("[controller]", controllerType.Name.Replace("Controller", "", StringComparison.Ordinal), StringComparison.OrdinalIgnoreCase)
            .Trim('/');

        return "/" + normalized;
    }

    private static string? GetMethodRouteTemplate(MethodInfo method)
    {
        foreach (var attr in method.GetCustomAttributes(inherit: true))
        {
            if (attr is HttpMethodAttribute http && !string.IsNullOrWhiteSpace(http.Template))
                return http.Template.Trim('/');
            if (attr is RouteAttribute route && !string.IsNullOrWhiteSpace(route.Template))
                return route.Template.Trim('/');
        }
        return null;
    }

    private static string CombineRoutes(string prefix, string? suffix)
    {
        if (string.IsNullOrWhiteSpace(suffix))
            return prefix;

        return $"{prefix.TrimEnd('/')}/{suffix.Trim('/')}";
    }

    private static IReadOnlyList<string> GetAuthPolicies(MemberInfo member)
    {
        return member.GetCustomAttributes<AuthorizeAttribute>(inherit: true)
            .SelectMany(a =>
            {
                if (!string.IsNullOrWhiteSpace(a.Policy))
                    return new[] { a.Policy };
                if (!string.IsNullOrWhiteSpace(a.Roles))
                    return a.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                return Array.Empty<string>();
            })
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string? GetHttpMethod(MethodInfo method)
    {
        if (method.GetCustomAttributes<HttpGetAttribute>(true).Any()) return "GET";
        if (method.GetCustomAttributes<HttpPostAttribute>(true).Any()) return "POST";
        if (method.GetCustomAttributes<HttpPutAttribute>(true).Any()) return "PUT";
        if (method.GetCustomAttributes<HttpDeleteAttribute>(true).Any()) return "DELETE";
        if (method.GetCustomAttributes<HttpPatchAttribute>(true).Any()) return "PATCH";
        return null;
    }
}
