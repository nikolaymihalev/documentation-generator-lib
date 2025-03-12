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

        public static string CsvRow = "Name,Type,Required";

        public static string MarkdownParametersRow = "| {0} | {1} | {2} |";

        public static string CsvParametersRow = "{0},{1},{2}";

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

        public static string CsvDescription = "{0},HTTP Method,{1},Route,`{2}`,Description,{3},Parameters,";
    }
}
