using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicsAPI.Model;

public class CountertopMessage
{
    public int Order { get; set; }

    public string Message { get; set; }

    public CountertopMessage(string msg, int order)
    {
        Message = msg;
        Order = order;
    }
}
