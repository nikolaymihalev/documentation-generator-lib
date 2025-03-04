using DocumentationLib.Common;
using DocumentationLib.Enums;

namespace DocumentationLib;

public static class DocumentationGenerator
{
    public static string ExportModelAsYaml<T>() where T : class
        => ModelDocumentGenerator.GenerateModel<T>(DocumentType.Yaml);

    public static string ExportModelAsJson<T>() where T : class
        => ModelDocumentGenerator.GenerateModel<T>(DocumentType.Json);

    public static string ExportModelAsMarkdown<T>() where T : class
        => ModelDocumentGenerator.GenerateText<T>(DocumentType.Markdown);

    public static string ExportModelAsCsv<T>() where T : class
        => ModelDocumentGenerator.GenerateText<T>(DocumentType.Csv);
}
