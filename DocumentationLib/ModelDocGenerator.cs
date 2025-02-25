using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using DocumentationLib.Constants;
using DocumentationLib.Enums;

namespace DocumentationLib;

internal static class ModelDocGenerator
{
    public static string GenerateInTextFormat<T>(DocFormat format, string xmlFilePath = "")
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
        
        var properties = modelType.GetProperties();

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

