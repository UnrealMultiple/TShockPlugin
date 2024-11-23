namespace Lagrange.XocMat.Adapter.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class ProgressMatch : Attribute
{
    public string Name { get; set; }

    public Type Type { get; set; }

    public string FieldName { get; set; }

    public ProgressMatch(string name, Type type, string fieldName)
    {
        this.Name = name;
        this.Type = type;
        this.FieldName = fieldName;
    }
}
