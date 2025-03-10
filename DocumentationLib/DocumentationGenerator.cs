using DocumentationLib.Common;
using DocumentationLib.Enums;

namespace DocumentationLib;

public static class DocumentationGenerator
{ 
    #region Export Model
    public static string ExportModelAsYaml<T>() where T : class
        => ModelDocumentGenerator.GenerateModel<T>(DocumentType.Yaml);

    public static string ExportModelAsJson<T>() where T : class
        => ModelDocumentGenerator.GenerateModel<T>(DocumentType.Json);

    public static string ExportModelAsMarkdown<T>() where T : class
        => ModelDocumentGenerator.GenerateText<T>(DocumentType.Markdown);

    public static string ExportModelAsCsv<T>() where T : class
        => ModelDocumentGenerator.GenerateText<T>(DocumentType.Csv);

    #endregion

    #region Save

    public static void SaveIntoFile(string[] documentations, string filePath)
        => ModelDocumentGenerator.GenerateFile(documentations, filePath, null, false);

    public static void SaveIntoFile(string[] documentations, string filePath, string fileName)
        => ModelDocumentGenerator.GenerateFile(documentations, filePath, fileName, false);

    public static void SaveIntoFile(string[] documentations, string filePath, bool append)
        => ModelDocumentGenerator.GenerateFile(documentations, filePath, null, append);

    public static void SaveIntoFile(string[] documentations, string filePath, string? fileName, bool append)
        => ModelDocumentGenerator.GenerateFile(documentations, filePath, fileName, append);

    #endregion 
}
