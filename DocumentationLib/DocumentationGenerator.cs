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

    public static string ExportArrayAsMarkdown(params Type[] types) 
        => ModelDocumentGenerator.GenerateTextFromArray(types, DocumentType.Markdown);

    public static string ExportArrayAsCsv(params Type[] types) 
        => ModelDocumentGenerator.GenerateTextFromArray(types, DocumentType.Csv);

    public static string ExportArrayAsYaml(params Type[] types)
        => ModelDocumentGenerator.GenerateModelFromArray(types, DocumentType.Yaml);

    public static string ExportArrayAsJson(params Type[] types)
        => ModelDocumentGenerator.GenerateModelFromArray(types, DocumentType.Json);
}
