using DocumentationLib.Enums;

namespace DocumentationLib.Common
{
    internal interface IDocumentationGenerator
    {
        static abstract string GenerateMarkdownOrCsvText<T>(DocumentType format);
        static abstract string GenerateJsonOrYmlText<T>(DocumentType format);
    }
}
