using DocumentationLib.Enums;
using DocumentationLib.Generators;

namespace DocumentationLib;

public static class DocumentationGenerator
{ 
    #region Export Model
    public static string ExportModelAsYaml<T>() where T : class
        => ModelDocumentаtionGenerator.GenerateJsonOrYmlText<T>(DocumentType.Yaml);

    public static string ExportModelAsJson<T>() where T : class
        => ModelDocumentаtionGenerator.GenerateJsonOrYmlText<T>(DocumentType.Json);

    public static string ExportModelAsMarkdown<T>() where T : class
        => ModelDocumentаtionGenerator.GenerateMarkdownOrCsvText<T>(DocumentType.Markdown);

    public static string ExportModelAsCsv<T>() where T : class
        => ModelDocumentаtionGenerator.GenerateMarkdownOrCsvText<T>(DocumentType.Csv);

    #endregion

    #region Save

    public static void SaveIntoFile(string[] documentations, string filePath)
        => ModelDocumentаtionGenerator.GenerateFile(documentations, filePath, null, false);

    public static void SaveIntoFile(string[] documentations, string filePath, string fileName)
        => ModelDocumentаtionGenerator.GenerateFile(documentations, filePath, fileName, false);

    public static void SaveIntoFile(string[] documentations, string filePath, bool append)
        => ModelDocumentаtionGenerator.GenerateFile(documentations, filePath, null, append);

    public static void SaveIntoFile(string[] documentations, string filePath, string? fileName, bool append)
        => ModelDocumentаtionGenerator.GenerateFile(documentations, filePath, fileName, append);

    #endregion 
}
