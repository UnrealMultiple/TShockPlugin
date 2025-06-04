using Economics.Skill.Attributes;
using Economics.Skill.Model;
using HookEvents.Terraria;
using Jint;
using Microsoft.Xna.Framework;
using System.Linq.Expressions;
using System.Reflection;
using TShockAPI;

namespace Economics.Skill.JSInterpreter;

public class JsScript(string Path)
{
    public string FilePathOrUri { get; set; } = Path;

    public int ReferenceCount { get; set; }

    public string Script { get; set; } = string.Empty;

    public List<string> PackageRequirements { get; set; } = [];
}