using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RolesModifying;

internal class TempPlayer : TSPlayer
{
    public string _Name { get; set; }
    public TempPlayer(string Name) : base(Name)
    {
        _Name = Name;
    }
}
