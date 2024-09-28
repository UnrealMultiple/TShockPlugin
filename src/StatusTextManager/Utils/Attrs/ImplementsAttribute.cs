namespace StatusTextManager.Utils.Attrs;

public class ImplementsAttribute : Attribute
{
    public Type[] ImplementsTypes;

    public ImplementsAttribute(params Type[] implementsTypes)
    {
        this.ImplementsTypes = implementsTypes;
    }
}