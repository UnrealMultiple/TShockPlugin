namespace Economics.Core.Model;

public class CountertopMessage
{
    public int Order { get; set; }

    public string Message { get; set; }

    public CountertopMessage(string msg, int order)
    {
        this.Message = msg;
        this.Order = order;
    }
}