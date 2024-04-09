using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicsAPI.Attributes;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class ProgressMap : Attribute
{
    public string Filed { get; set; }

    public object value { get; set; }

    public ProgressMap(string Filed, object value)
    {
        this.Filed = Filed;

        this.value = value;
    }
}

