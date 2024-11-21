namespace Lagrange.XocMat.Adapter.Attributes;

[AttributeUsage(AttributeTargets.Method)]
internal class RestMatch : Attribute
{
    public string ApiPath { get; set; }

    public RestMatch(string api)
    {
        this.ApiPath = api;
    }
}
