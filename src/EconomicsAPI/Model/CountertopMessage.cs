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
