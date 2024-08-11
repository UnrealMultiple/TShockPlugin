using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Economics.Skill.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class JavaScriptFunction : Attribute
{
    public string Name { get; set; }

    public JavaScriptFunction(string name)
    {
        Name = name;
    }
}
