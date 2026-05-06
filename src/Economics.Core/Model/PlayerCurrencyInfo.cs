namespace Economics.Core.Model;

public class PlayerCurrencyInfo
{
    public long Number { get; set; }
    public string CurrencyType { get; set; } = "";
    public string PlayerName { get; set; } = "";

    public override string ToString()
    {
        return $"{this.CurrencyType}x{this.Number}";
    }
}
