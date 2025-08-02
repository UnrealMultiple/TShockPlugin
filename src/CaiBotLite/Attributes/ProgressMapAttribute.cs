namespace CaiBotLite.Attributes;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class ProgressMapAttribute(string fieldName, object expectedValue) : Attribute
{
    public string FieldName { get; } = fieldName;
    public object ExpectedValue { get; } = expectedValue;
}