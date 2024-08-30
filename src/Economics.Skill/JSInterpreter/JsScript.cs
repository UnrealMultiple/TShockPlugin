namespace Economics.Skill.JSInterpreter;

public class JsScript
{
    public string FilePathOrUri { get; set; } = string.Empty;

    public int ReferenceCount { get; set; }

    public string Script { get; set; } = string.Empty;

    public List<string> PackageRequirements { get; set; }

    public JsScript()
    {
        PackageRequirements = new List<string>();
    }
}
