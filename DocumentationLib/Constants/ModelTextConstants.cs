namespace DocumentationLib.Constants;

internal static class ModelTextConstants
{  
    public static string MarkdownHeader = "# {0} Model" + Environment.NewLine + 
                Environment.NewLine + 
                "| Property | Type | Description | Attributes | Value |" + Environment.NewLine +
                "|----------|------|-------------|------------|-------|" + Environment.NewLine;

    public static string CsvHeader = "{0} Model" + 
        Environment.NewLine + 
        "Property,Type,Description,Attributes,Value" + 
        Environment.NewLine;

    public static string CsvRow = "{0},{1},{2},{3},{4}";

    public static string MarkdownRow = "| {0} | {1} | {2} | {3} | {4} |";
}