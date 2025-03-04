using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using DocumentationLib.Constants;
using DocumentationLib.Enums;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DocumentationLib.Common;

internal static class ModelDocumentGenerator
{
    #region Generate From Single Model
    public static string GenerateText<T>(DocumentType format)
    {
        Type modelType = typeof(T);

        var stringBuilder = new StringBuilder();
        
        switch (format)
        {
            case DocumentType.Markdown:
                stringBuilder.AppendLine(string.Format(FormatTextConstants.MarkdownHeader, modelType.Name)); break;
            case DocumentType.Csv:
                stringBuilder.AppendLine(FormatTextConstants.CsvHeader); break;
        }
        
        stringBuilder.AppendLine(GetText(modelType, format, stringBuilder));

        return stringBuilder.ToString();
    }

    public static string GenerateModel<T>(DocumentType format)
    {
        Type modelType = typeof(T);
        
        var result = GetModel(modelType);

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

    #endregion

    #region Generate From Multiple Models
    public static string GenerateTextFromArray(Type[] types, DocumentType format)
    {
        var stringBuilder = new StringBuilder();

        foreach(var type in types)
        {
            stringBuilder.AppendLine(GetText(type, format));
        }

        return stringBuilder.ToString().Trim();
    }

    #endregion

    #region Private Methods

    private static string GetDefaultValue(Type modelType, PropertyInfo property)
    {
        try
        {
            var constructor = modelType.GetConstructor(Type.EmptyTypes);
            if (constructor is null)
                return "-";

            var instance = Activator.CreateInstance(modelType);
            var value = property.GetValue(instance);

            if (value is null || value.ToString() is null) 
                return "null";
                
            if (value is string strValue) 
                return $"\"{strValue}\"";
                
            if (value is bool boolValue) 
                return boolValue ? "true" : "false";
                
            return value.ToString()!;
        }
        catch
        {
            return "-";
        }
    }

    private static string GetPropertyDescription(Type modelType, PropertyInfo property)
    {
        var description = property.GetCustomAttribute<DocumentationAttribute>()?.Summary ?? null;

        if (!string.IsNullOrEmpty(description))
            return description;

        return "-";
    }

    private static string GetXmlSummary(string xmlFilePath, Type modelType, PropertyInfo property)
    {
        if (File.Exists(xmlFilePath))
        {
            XDocument _xmlDocumentation = XDocument.Load(xmlFilePath);

            string memberName = $"P:{modelType.FullName}.{property.Name}";
            var memberNode = _xmlDocumentation.Descendants("member")
                .FirstOrDefault(x => x.Attribute("name")?.Value == memberName);

            return memberNode?.Element("summary")?.Value.Trim()!;
        }

        return string.Empty;
    }

    private static object GetModel(Type modelType)
    {
        var properties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        return new {
            ModelName = modelType.Name,
            Properties = properties.Select(prop => new {
                Name = prop.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? prop.Name ?? "-",
                Type = prop.PropertyType.Name,
                Description = GetPropertyDescription(modelType, prop),
                Attributes = GetPropertyAttributes(prop),
                DefaultValue = GetDefaultValue(modelType, prop)
            })
        };
    }

    private static string  GetPropertyAttributes(PropertyInfo prop)
    {
        var attributes = prop.GetCustomAttributes()
            .Select(attr =>
            {
                var attrType = attr.GetType();
                var attrName = attrType.Name.Replace("Attribute", ""); 
                
                var constructor = attrType.GetConstructors().FirstOrDefault();
                var arguments = constructor != null
                    ? constructor.GetParameters()
                        .Select(p => attrType.GetProperty(p.Name!)?.GetValue(attr)?.ToString())
                        .Where(v => v != null)
                        .ToList()!
                    : new List<string>();

                return arguments.Any()
                    ? $"{attrName}({string.Join(", ", arguments)})"
                    : attrName;
            })
            .ToList();

        return attributes.Any() ? string.Join(", ", attributes) : "-";
    }

    private static string GetText(Type modelType, DocumentType format)
    {
        var properties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            string propName = prop.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? prop.Name ?? "-";

            string typeName = prop.PropertyType.Name;
            
            string description = GetPropertyDescription(modelType, prop);

            string value = GetDefaultValue(modelType, prop);

            string attributes = GetPropertyAttributes(prop);

            switch (format)
            {
                case DocumentType.Markdown:
                    return string.Format(FormatTextConstants.MarkdownRow, propName, typeName, description, attributes, value);
                case DocumentType.Csv:
                    return string.Format(FormatTextConstants.CsvRow, propName, typeName, description, attributes, value);
            }
        }

        return string.Empty;
    }
    #endregion
}