using Economics.Core.ConfigFiles;
using Economics.Core.Model;

namespace Economics.Core.Services;

public interface IExchangeService
{
    Result ValidateExchange(string fromCurrency, string toCurrency, long amount, Dictionary<string, long> userBalances);

    Result<ExchangeResult> ExecuteExchange(string playerName, string fromCurrency, string toCurrency, long amount);

    Result<ExchangeCalculation> CalculateExchange(string fromCurrency, string toCurrency, long amount);

    string GetExchangePreview(string fromCurrency, string toCurrency, long amount, Dictionary<string, long> userBalances);

    string GetExchangeOptions(string currencyName);

    string GetCurrencyGraph();

    IReadOnlyList<ArbitrageCycle> GetDetectedCycles();

    Result<CurrencyDefinition> GetCurrency(string name);

    IReadOnlyCollection<CurrencyDefinition> GetAllCurrencies();
}

public class ExchangeResult
{
    public string FromCurrency { get; set; } = "";
    public string ToCurrency { get; set; } = "";
    public long FromAmount { get; set; }
    public long ToAmount { get; set; }
}

public class ExchangeCalculation
{
    public long ResultAmount { get; set; }
    public long ExchangeRate { get; set; }
}
