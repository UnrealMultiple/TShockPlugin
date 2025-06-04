using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace Economics.Core.Command;

[AttributeUsage(AttributeTargets.Method)]
public class CommandPermissionAttribute(params string[] perm) : Attribute
{
    public string[] Permissions = perm;

    public bool DetectAll = false;

    public bool DetectPermission(TSPlayer ply)
    {
        return this.DetectAll ? this.Permissions.All(ply.HasPermission) : this.Permissions.Any(ply.HasPermission);
    }
}
