namespace Economics.Skill.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class JavaScriptFunction : Attribute
{
    public string Name { get; set; }

    public JavaScriptFunction(string name)
    {
        this.Name = name;
    }
}