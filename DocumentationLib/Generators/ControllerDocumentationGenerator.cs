using DocumentationLib;
using DocumentationLib.Common;
using DocumentationLib.Constants;
using DocumentationLib.Enums;
using System.Reflection;
using System.Text;
using System.Text.Json;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace DocumentationLib.Generators;

internal class ControllerDocumentationGenerator : IDocumentationGenerator
{
    public static string GenerateMarkdownOrCsvText<T>(DocumentType format)
    {
        Type controllerType = typeof(T);
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
            stringBuilder.AppendLine(GetMethodText(method, routePrefix, format));

            stringBuilder.AppendLine(GetParametersText(method, format));            

            stringBuilder.AppendLine();
        }

        return stringBuilder.ToString().Trim();
    }

    public static string GenerateJsonOrYmlText<T>(DocumentType format)
    {
        Type modelType = typeof(T);

        return format switch 
        {
            DocumentType.Json => JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }),
            DocumentType.Yaml => new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build()
                .Serialize(result),
            _ => string.Empty
        };
    }

    #region Private Methods
    private static string GetMethodText(MethodInfo method, string routePrefix, DocumentType format)
    {
        string route = GetRoute(routePrefix, method);
        string httpMethod = GetHttpMethod(method);
        string description = GetMethodDescription(method);

        return format switch
        {
            DocumentType.Markdown => string.Format(ControllerTextConstants.MarkdownDescription, method.Name, httpMethod, route, description),
            DocumentType.Csv => string.Format(ControllerTextConstants.CsvDescription, method.Name, httpMethod, route, description),
            _ => string.Format(ControllerTextConstants.MarkdownDescription, method.Name, httpMethod, route, description)
        };
    }

    private static string GetParametersText(MethodInfo method, DocumentType format)
    {
        string result = "";

        if (method.GetParameters().Any())
        {
            if(format == DocumentType.Csv)
            {
                result += ControllerTextConstants.CsvRow;

                foreach (var param in method.GetParameters())
                {
                    string paramName = param.Name!;
                    string paramType = param.ParameterType.Name;
                    string isRequired = param.HasDefaultValue ? YesNo.No.ToString() : YesNo.Yes.ToString();

                    result += string.Format(ControllerTextConstants.CsvParametersRow, paramName, paramType, isRequired);
                }
            }
            else if(format == DocumentType.Markdown)
            {
                result += ControllerTextConstants.MarkdownRow;

                foreach (var param in method.GetParameters())
                {
                    string paramName = param.Name!;
                    string paramType = param.ParameterType.Name;
                    string isRequired = param.HasDefaultValue ? YesNo.No.ToString() : YesNo.Yes.ToString();

                    result += string.Format(ControllerTextConstants.MarkdownParametersRow, paramName, paramType, isRequired);
                }
            }
        }
        else
        {
            result = "-";
        }

        return result;
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
