using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace ModelDocGenerator;

public static class ModelDocGenerator
{

    public static string Generate(Type modelType)
    {
        var stringBuilder = new StringBuilder();
        
        stringBuilder.AppendLine($"# {modelType.Name} Model");
        stringBuilder.AppendLine();
        stringBuilder.AppendLine("| Property | Type | Description | Value |");
        stringBuilder.AppendLine("|----------|------|-------------|-------|");
        
        var properties = modelType.GetProperties();

        foreach (var prop in properties)
        {
            string propName = prop.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? prop.Name ?? "-";

            string typeName = prop.PropertyType.Name;
            
            string description = prop.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "-";

            string value = GetDefaultValue(modelType, prop);

            stringBuilder.AppendLine($"| {propName} | {typeName} | {description} | {value} |");
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
}
