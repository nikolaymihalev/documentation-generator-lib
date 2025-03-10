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
        => GenerateFile(documentations, filePath, null, false);

    public static void SaveIntoFile(string[] documentations, string filePath, string fileName)
        => GenerateFile(documentations, filePath, fileName, false);

    public static void SaveIntoFile(string[] documentations, string filePath, bool append)
        => GenerateFile(documentations, filePath, null, append);

    public static void SaveIntoFile(string[] documentations, string filePath, string? fileName, bool append)
        => GenerateFile(documentations, filePath, fileName, append);

    #endregion

    #region Private Methods
    private static void GenerateFile(string[] documentations, string filePath, string? fileName = null, bool append = false)
    {
        if (string.IsNullOrEmpty(fileName))
            fileName = "model_documentation.txt";

        Directory.CreateDirectory(filePath);

        string fullPath = Path.Combine(filePath, fileName!);

        using (StreamWriter sw = new StreamWriter(fullPath, append))
        {
            foreach (var documentation in documentations)
            {
                sw.WriteLine(documentation);
            }
        }
    }
    #endregion
}
