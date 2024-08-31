namespace EconomicsAPI.Attributes;

[AttributeUsage(AttributeTargets.Method)]
internal class RestMap : Attribute
{
    public string ApiPath { get; set; }

    public RestMap(string api)
    {
        this.ApiPath = api;
    }
}