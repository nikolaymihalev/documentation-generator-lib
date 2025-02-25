namespace DocumentationLib;

internal static class FormatTextConstants
{  
    public static string MarkdownHeader = "# {0} Model" + Environment.NewLine + 
                Environment.NewLine + 
                "| Property | Type | Description | Value |" + Environment.NewLine +
                "|----------|------|-------------|-------|";

    public static string CsvHeader = "Property,Type,Description,Value";
}