using DocumentationLib;
using DocumentationLib.Constants;
using DocumentationLib.Enums;
using System.Reflection;
using System.Text;

namespace DocumentationLib.Generators;

internal static class ControllerDocumentationGenerator
{
    public static string GenerateTextDocs<TController>(DocumentType format)
    {
        Type controllerType = typeof(TController);
        StringBuilder stringBuilder = new StringBuilder();

        string routePrefix = GetControllerRoutePrefix(controllerType);

        switch (format)
        {
            case DocumentType.Markdown:
                stringBuilder.AppendLine(string.Format(ControllerTextConstants.MarkdownHeader, controllerType.Name)); break;
            case DocumentType.Csv:
                stringBuilder.AppendLine(string.Format(ControllerTextConstants.CsvHeader, controllerType.Name)); break;
        }


        var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        foreach (var method in methods)
        {
            stringBuilder.AppendLine(GetText(method, routePrefix));

            if (method.GetParameters().Any())
            {
                stringBuilder.AppendLine("| Name | Type | Required |");
                stringBuilder.AppendLine("|------|------|----------|");

                foreach (var param in method.GetParameters())
                {
                    string paramName = param.Name;
                    string paramType = param.ParameterType.Name;
                    string isRequired = param.HasDefaultValue ? "No" : "Yes";

                    stringBuilder.AppendLine($"| {paramName} | {paramType} | {isRequired} |");
                }
            }
            else
            {
                stringBuilder.AppendLine("  - No parameters.");
            }

            stringBuilder.AppendLine();
        }

        return stringBuilder.ToString();
    }

    #region Private Methods
    private static string GetText(MethodInfo method, string routePrefix)
    {

        string route = GetRoute(routePrefix, method);
        string httpMethod = GetHttpMethod(method);
        string description = GetMethodDescription(method);

        return $"## {method.Name}" +
            Environment.NewLine +
            $"- **HTTP Method:** {httpMethod}" +
            Environment.NewLine +
            $"- **Route:** `{route}`" +
            Environment.NewLine +
            $"- **Description:** {description}" +
            Environment.NewLine +
            $"- **Parameters:**";
    }

    private static string GetControllerRoutePrefix(Type controllerType)
    {
        var routeAttr = controllerType.GetCustomAttributes()
            .FirstOrDefault(a => a.GetType().Name == "RouteAttribute");

        if (routeAttr != null)
        {
            var templateProperty = routeAttr.GetType().GetProperty("Template");
            return templateProperty?.GetValue(routeAttr)?.ToString() ?? "";
        }

        return "";
    }

    private static string GetRoute(string routePrefix, MethodInfo method)
    {
        var routeAttr = method.GetCustomAttributes()
            .FirstOrDefault(a => a.GetType().Name.StartsWith("Http"));

        string methodRoute = routeAttr != null ? method.Name : "";

        if (!string.IsNullOrEmpty(routePrefix))
        {
            return $"/{routePrefix}/{methodRoute}".Replace("//", "/");
        }

        return $"/{method.DeclaringType?.Name.Replace("Controller", "")}/{methodRoute}";
    }

    private static string GetHttpMethod(MethodInfo method)
    {
        var httpAttr = method.GetCustomAttributes()
            .FirstOrDefault(a => a.GetType().Name.StartsWith("Http"));

        return httpAttr?.GetType().Name.Replace("Attribute", "").ToUpper() ?? "UNKNOWN";
    }

    private static string GetMethodDescription(MethodInfo method)
    {
        var description = method.GetCustomAttribute<DocumentationAttribute>()?.Summary ?? null;

        if (!string.IsNullOrEmpty(description))
            return description;

        return "-"; 
    }

    #endregion
}
