using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using DocumentationLib.Constants;
using DocumentationLib.Enums;

namespace DocumentationLib;

internal static class ModelDocGenerator
{
    public static string GenerateText<T>(DocFormat format, string xmlFilePath = "")
    {
        var modelType = typeof(T);

        var stringBuilder = new StringBuilder();
        
        switch (format)
        {
            case DocFormat.Markdown:
                stringBuilder.AppendLine(string.Format(FormatTextConstants.MarkdownHeader, modelType.Name)); break;
            case DocFormat.Csv:
                stringBuilder.AppendLine(FormatTextConstants.CsvHeader); break;
        }
        
        var properties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            string propName = prop.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? prop.Name ?? "-";

            string typeName = prop.PropertyType.Name;
            
            string description = GetPropertyDescription(modelType, prop, xmlFilePath);

            string value = GetDefaultValue(modelType, prop);

            switch (format)
            {
                case DocFormat.Markdown:
                    stringBuilder.AppendLine(string.Format(FormatTextConstants.MarkdownRow, propName, typeName, description, value)); break;
                case DocFormat.Csv:
                    stringBuilder.AppendLine(string.Format(FormatTextConstants.CsvRow, propName, typeName, description, value)); break;
            }
        }

        return stringBuilder.ToString();
    }

    public static string GenerateModel<T>(DocFormat format, string xmlFilePath = "")
    {
        var modelType = typeof(T);

        var properties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        if(format == DocFormat.Json)
        {
            var jsonModel = new {
                ModelName = modelType.Name,
                Properties = properties.Select(prop => new {
                    Name = prop.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? prop.Name ?? "-",
                    Type = prop.PropertyType.Name,
                    Description = GetPropertyDescription(modelType, prop, xmlFilePath),
                    DefaultValue = GetDefaultValue(modelType, prop)
                })
            };


            return JsonSerializer.Serialize(jsonModel, new JsonSerializerOptions { WriteIndented = true });
        }        
    }

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
}
