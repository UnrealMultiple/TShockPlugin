using TShockAPI;

namespace RolesModifying;

internal class TempPlayer : TSPlayer
{
    public string _Name { get; set; }
    public TempPlayer(string Name) : base(Name)
    {
        this._Name = Name;
    }
}