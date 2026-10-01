public class TemplateInfo
{
    public string Id { get; set; }          // Language id
    public string Name { get; set; }        // Display name
    public string Extension { get; set; }   // ".cs", ".cpp", ".html"
    public string Language { get; set; }    // "C#", "C++", "HTML" etc
    public string Description { get; set; } // Description for display in the list

    public override string ToString()
    {
        return Name;
    }
}
