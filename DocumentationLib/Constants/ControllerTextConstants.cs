using System.Net.Http;

namespace DocumentationLib.Constants
{
    internal static class ControllerTextConstants
    {
        public static string MarkdownHeader = "# {0} Controller";

        public static string CsvHeader = "{0} Controller";

        public static string MarkdownRow = "| Name | Type | Required |" +
                    Environment.NewLine +
                    "|------|------|----------|" +
                    Environment.NewLine;

        public static string MarkdownParametersRow = "| {0} | {1} | {2} |";

        public static string MarkdownDescription = 
            "## {0}" +
            Environment.NewLine +
            "- **HTTP Method:** {1}" +
            Environment.NewLine +
            "- **Route:** `{2}`" +
            Environment.NewLine +
            "- **Description:** {3}" +
            Environment.NewLine +
            "- **Parameters:**";
    }
}
