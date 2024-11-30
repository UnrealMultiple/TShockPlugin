using System.ComponentModel;

namespace System.Runtime.CompilerServices;
#if !NET7_0_OR_GREATER

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
public sealed class CompilerFeatureRequiredAttribute : Attribute
{
    public const string RefStructs = "RefStructs";

    public const string RequiredMembers = "RequiredMembers";

    public string FeatureName { get; }

    public bool IsOptional { get; init; }

    public CompilerFeatureRequiredAttribute(string featureName)
    {
        this.FeatureName = featureName;
    }
}
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class RequiredMemberAttribute : Attribute
{
}
#endif