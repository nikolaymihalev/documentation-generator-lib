using System.Reflection;
using System.Text;

namespace DocumentationLib.Common
{
    internal static class ControllerDocumentGenerator
    {
        public static string GenerateControllerDocs<TController>()
        {
            Type controllerType = typeof(TController);
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"# {controllerType.Name} Controller");

            var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var method in methods)
            {
                string route = GetRoute(method);
                string httpMethod = GetHttpMethod(method);
                string description = GetDescription(method);

                stringBuilder.AppendLine($"## {method.Name}");
                stringBuilder.AppendLine($"- **HTTP Method:** {httpMethod}");
                stringBuilder.AppendLine($"- **Route:** `{route}`");
                stringBuilder.AppendLine($"- **Description:** {description}");
                stringBuilder.AppendLine($"- **Parameters:**");

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

        private static string GetRoute(MethodInfo method)
        {
            var routeAttr = method.GetCustomAttributes()
                .FirstOrDefault(a => a.GetType().Name.StartsWith("Http"));
            return routeAttr != null ? $"/{method.DeclaringType?.Name.Replace("Controller", "")}/{method.Name}" : "N/A";
        }

        private static string GetHttpMethod(MethodInfo method)
        {
            var httpAttr = method.GetCustomAttributes()
                .FirstOrDefault(a => a.GetType().Name.StartsWith("Http"));

            return httpAttr?.GetType().Name.Replace("Attribute", "").ToUpper() ?? "UNKNOWN";
        }
    }
}
