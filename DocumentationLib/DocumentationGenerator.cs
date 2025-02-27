using DocumentationLib.Common;
using DocumentationLib.Enums;

namespace DocumentationLib;

public static class DocumentationGenerator
{
    public static string ExportModelAsYaml<T>() 
        => ModelDocumentGenerator.GenerateModel<T>(DocumentType.Yaml);

    public static string ExportModelAsJson<T>()
        => ModelDocumentGenerator.GenerateModel<T>(DocumentType.Json);

    public static string ExportModelAsMarkdown<T>()
        => ModelDocumentGenerator.GenerateText<T>(DocumentType.Markdown);

    public static string ExportModelAsCsv<T>()
        => ModelDocumentGenerator.GenerateText<T>(DocumentType.Csv);

    public static string ExportModelAsYaml<T>(string xmlFilePath) 
        => ModelDocumentGenerator.GenerateModel<T>(DocumentType.Yaml, xmlFilePath);

    public static string ExportModelAsJson<T>(string xmlFilePath)
        => ModelDocumentGenerator.GenerateModel<T>(DocumentType.Json, xmlFilePath);

    public static string ExportModelAsMarkdown<T>(string xmlFilePath)
        => ModelDocumentGenerator.GenerateText<T>(DocumentType.Markdown, xmlFilePath);

    public static string ExportModelAsCsv<T>(string xmlFilePath)
        => ModelDocumentGenerator.GenerateText<T>(DocumentType.Csv, xmlFilePath);
}
