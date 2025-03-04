namespace DocumentationLib;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class DocumentationAttribute : Attribute
{
    public string Summary { get; set; }
    public DocumentationAttribute(string summary) => Summary = summary;
}
