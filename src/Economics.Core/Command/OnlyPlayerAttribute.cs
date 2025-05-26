using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Economics.Core.Command;

[AttributeUsage(AttributeTargets.Method)]
public class OnlyPlayerAttribute : Attribute
{
}
