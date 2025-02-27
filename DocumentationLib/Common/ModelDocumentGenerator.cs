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
    public static string GenerateText<T>(DocumentType format, string xmlFilePath = "")
    {
        var modelType = typeof(T);

        var stringBuilder = new StringBuilder();
        
        switch (format)
        {
            case DocumentType.Markdown:
                stringBuilder.AppendLine(string.Format(FormatTextConstants.MarkdownHeader, modelType.Name)); break;
            case DocumentType.Csv:
                stringBuilder.AppendLine(FormatTextConstants.CsvHeader); break;
        }
        
        var properties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            string propName = prop.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? prop.Name ?? "-";

            string typeName = prop.PropertyType.Name;
            
            string description = GetPropertyDescription(modelType, prop, xmlFilePath);

            string value = GetDefaultValue(modelType, prop);

            var attributes = GetPropertyAttributes(prop);

            switch (format)
            {
                case DocumentType.Markdown:
                    stringBuilder.AppendLine(string.Format(FormatTextConstants.MarkdownRow, propName, typeName, description, attributes, value)); break;
                case DocumentType.Csv:
                    stringBuilder.AppendLine(string.Format(FormatTextConstants.CsvRow, propName, typeName, description, attributes, value)); break;
            }
        }

        return stringBuilder.ToString();
    }

    public static string GenerateModel<T>(DocumentType format, string xmlFilePath = "")
    {
        var modelType = typeof(T);
        
        var result = GetModel(modelType, xmlFilePath);

        if(format == DocumentType.Json)
            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        else if(format == DocumentType.Yaml)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            return serializer.Serialize(result);
        }
        else
            return string.Empty;
    }

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

    private static string GetPropertyDescription(Type modelType, PropertyInfo property, string xmlFilePath)
    {
        var description = property.GetCustomAttribute<DescriptionAttribute>()?.Description ?? null;

        if (!string.IsNullOrEmpty(description))
            return description;

        if (!string.IsNullOrEmpty(xmlFilePath))
            return GetXmlSummary(xmlFilePath ,modelType, property);

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

    private static object GetModel(Type modelType, string xmlFilePath)
    {
        var properties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        return new {
            ModelName = modelType.Name,
            Properties = properties.Select(prop => new {
                Name = prop.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? prop.Name ?? "-",
                Type = prop.PropertyType.Name,
                Description = GetPropertyDescription(modelType, prop, xmlFilePath),
                Attributes = GetPropertyAttributes(prop),
                DefaultValue = GetDefaultValue(modelType, prop)
            })
        };
    }

    private static List<string> GetPropertyAttributes(PropertyInfo prop)
        => prop.GetCustomAttributes()
                .Select(attr =>
                {
                    var attrType = attr.GetType();
                    var attrName = attrType.Name.Replace("Attribute", ""); 
                    
                    var parameters = attrType.GetConstructors()
                            .FirstOrDefault()?
                            .GetParameters()
                            .Select(p => p.Name)
                            .ToList();

                    return parameters != null && parameters.Count > 0
                        ? $"{attrName}({string.Join(", ", parameters)})"
                        : attrName;
                })
                .ToList();

    #endregion
}