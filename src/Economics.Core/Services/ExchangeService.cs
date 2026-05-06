using Economics.Core.ConfigFiles;
using Economics.Core.Model;
using System.Text;

namespace Economics.Core.Services;

internal class ExchangeService : IExchangeService
{
    private readonly ICurrencyService _currencyService;
    private readonly List<ArbitrageCycle> _detectedCycles = [];

    public ExchangeService(ICurrencyService currencyService)
    {
        this._currencyService = currencyService ?? throw new ArgumentNullException(nameof(currencyService));
        this.ValidateAndDetectCycles();
    }

    private void ValidateAndDetectCycles()
    {
        foreach (var currency in Setting.Instance.Currencies)
        {
            currency.CanBeObtainedByExchange = false;
        }

        foreach (var currency in Setting.Instance.Currencies)
        {
            foreach (var targetName in currency.ExchangeRates.Keys)
            {
                var target = Setting.Instance.GetCurrency(targetName);
                target!.CanBeObtainedByExchange = true;
            }
        }

        this._detectedCycles.Clear();
        var cycles = DetectArbitrageCycles();
        this._detectedCycles.AddRange(cycles);
    }

    private static List<ArbitrageCycle> DetectArbitrageCycles()
    {
        var cycles = new List<ArbitrageCycle>();
        var visited = new HashSet<string>();

        foreach (var currency in Setting.Instance.Currencies)
        {
            if (!visited.Contains(currency.Name))
            {
                FindCyclesFromNode(currency.Name, [], [], cycles, visited);
            }
        }

        return cycles;
    }

    private static void FindCyclesFromNode(
        string current,
        List<string> path,
        List<decimal> rates,
        List<ArbitrageCycle> cycles,
        HashSet<string> visited)
    {
        var cycleStart = path.IndexOf(current);
        if (cycleStart >= 0 && path.Count > 0)
        {
            var cyclePath = path.Skip(cycleStart).ToList();
            cyclePath.Add(current);

            var cycleRates = rates.Skip(cycleStart).ToList();

            decimal multiplier = 1;
            foreach (var rate in cycleRates)
            {
                multiplier *= rate;
            }

            var cycleKey = string.Join(",", cyclePath);
            if (!cycles.Any(c => string.Join(",", c.CyclePath) == cycleKey))
            {
                cycles.Add(new ArbitrageCycle
                {
                    CyclePath = cyclePath,
                    Rates = cycleRates,
                    ArbitrageMultiplier = multiplier
                });
            }

            return;
        }

        var currency = Setting.Instance.GetCurrency(current);
        if (currency == null)
        {
            return;
        }

        path.Add(current);
        visited.Add(current);

        foreach (var (targetName, rate) in currency.ExchangeRates)
        {
            if (rate <= 0)
            {
                continue;
            }

            rates.Add(rate);
            FindCyclesFromNode(targetName, path, rates, cycles, visited);
            rates.RemoveAt(rates.Count - 1);
        }

        path.RemoveAt(path.Count - 1);
    }

    public Result ValidateExchange(string fromCurrency, string toCurrency, long amount, Dictionary<string, long> userBalances)
    {
        if (string.IsNullOrWhiteSpace(fromCurrency))
        {
            return Result.Failure(GetString("源货币名称不能为空"));
        }

        if (string.IsNullOrWhiteSpace(toCurrency))
        {
            return Result.Failure(GetString("目标货币名称不能为空"));
        }

        if (amount <= 0)
        {
            return Result.Failure(GetString("兑换数量必须大于0"));
        }

        var source = Setting.Instance.GetCurrency(fromCurrency);
        if (source == null)
        {
            return Result.Failure(GetString($"源货币 '{fromCurrency}' 不存在"));
        }

        var target = Setting.Instance.GetCurrency(toCurrency);
        if (target == null)
        {
            return Result.Failure(GetString($"目标货币 '{toCurrency}' 不存在"));
        }

        if (!source.ExchangeRates.TryGetValue(toCurrency, out _))
        {
            return Result.Failure(GetString($"'{fromCurrency}' 不能直接兑换为 '{toCurrency}'"));
        }

        var userHas = userBalances.GetValueOrDefault(fromCurrency, 0);
        return userHas < amount ? Result.Failure(GetString($"{fromCurrency} 不足: 需要 {amount}, 拥有 {userHas}, 缺 {amount - userHas}")) : Result.Success();
    }

    public Result<ExchangeResult> ExecuteExchange(string playerName, string fromCurrency, string toCurrency, long amount)
    {
        var balancesResult = this._currencyService.GetAllBalances(playerName);
        if (balancesResult.IsFailure)
        {
            return Result.Failure<ExchangeResult>(balancesResult.Error!);
        }

        var validationResult = this.ValidateExchange(fromCurrency, toCurrency, amount, balancesResult.Value!);
        if (validationResult.IsFailure)
        {
            return Result.Failure<ExchangeResult>(validationResult.Error!);
        }

        var calculationResult = this.CalculateExchange(fromCurrency, toCurrency, amount);
        if (calculationResult.IsFailure)
        {
            return Result.Failure<ExchangeResult>(calculationResult.Error!);
        }

        var calculation = calculationResult.Value!;

        var deductResult = this._currencyService.DeductCurrency(playerName, fromCurrency, amount);
        if (deductResult.IsFailure)
        {
            return Result.Failure<ExchangeResult>(GetString($"兑换失败: {deductResult.Error}"));
        }

        var addResult = this._currencyService.AddCurrency(playerName, toCurrency, calculation.ResultAmount);
        if (addResult.IsFailure)
        {
            this._currencyService.AddCurrency(playerName, fromCurrency, amount);
            return Result.Failure<ExchangeResult>(GetString($"兑换失败，已回滚: {addResult.Error}"));
        }

        return Result.Success(new ExchangeResult
        {
            FromCurrency = fromCurrency,
            ToCurrency = toCurrency,
            FromAmount = amount,
            ToAmount = calculation.ResultAmount
        });
    }

    public Result<ExchangeCalculation> CalculateExchange(string fromCurrency, string toCurrency, long amount)
    {
        var source = Setting.Instance.GetCurrency(fromCurrency);
        if (source == null)
        {
            return Result.Failure<ExchangeCalculation>(GetString($"源货币 '{fromCurrency}' 不存在"));
        }

        if (!source.ExchangeRates.TryGetValue(toCurrency, out var rate))
        {
            return Result.Failure<ExchangeCalculation>(GetString($"'{fromCurrency}' 不能直接兑换为 '{toCurrency}'"));
        }

        try
        {
            checked
            {
                var result = amount * rate;
                return Result.Success(new ExchangeCalculation
                {
                    ResultAmount = result,
                    ExchangeRate = rate
                });
            }
        }
        catch (OverflowException)
        {
            return Result.Failure<ExchangeCalculation>(GetString("兑换结果超出数值范围"));
        }
    }

    public string GetExchangePreview(string fromCurrency, string toCurrency, long amount, Dictionary<string, long> userBalances)
    {
        var source = Setting.Instance.GetCurrency(fromCurrency);
        var target = Setting.Instance.GetCurrency(toCurrency);

        if (source == null || target == null)
        {
            return GetString("货币不存在");
        }

        var sb = new StringBuilder();
        sb.AppendLine(GetString($"[兑换预览] {fromCurrency} → {toCurrency}"));

        if (!source.ExchangeRates.TryGetValue(toCurrency, out var rate))
        {
            sb.AppendLine(GetString($"'{fromCurrency}' 不能直接兑换为 '{toCurrency}'"));
            sb.AppendLine(GetString($"'{fromCurrency}' 可以兑换为:"));
            foreach (var (targetName, r) in source.ExchangeRates)
            {
                sb.AppendLine(GetString($"  - {targetName} (1:{r})"));
            }
            return sb.ToString();
        }

        var userHas = userBalances.GetValueOrDefault(fromCurrency, 0);
        var resultAmount = amount * rate;
        var status = userHas >= amount ? GetString("[c/00FF00:✓ 可兑换]") : GetString($"[c/FF0000:✗ 缺{amount - userHas}]");

        sb.AppendLine(GetString($"兑换比例: 1 {fromCurrency} = {rate} {toCurrency}"));
        sb.AppendLine(GetString($"兑换: {amount} {fromCurrency} → {resultAmount} {toCurrency}"));
        sb.AppendLine(GetString($"拥有: {userHas} {fromCurrency} {status}"));
        return sb.ToString();
    }

    public string GetExchangeOptions(string currencyName)
    {
        var currency = Setting.Instance.GetCurrency(currencyName);
        if (currency == null)
        {
            return GetString($"货币 '{currencyName}' 不存在");
        }

        var sb = new StringBuilder();
        sb.AppendLine(GetString($"=== {currencyName} 的兑换选项 ==="));
        if (currency.CanExchangeToOthers)
        {
            sb.AppendLine(GetString($"\n可以将 {currencyName} 兑换为:"));
            foreach (var (targetName, rate) in currency.ExchangeRates)
            {
                sb.AppendLine(GetString($"  → {targetName} (1:{rate})"));
            }
        }
        else
        {
            sb.AppendLine(GetString($"\n{currencyName} 无法兑换为其他货币"));
        }

        var sources = GetExchangePathsTo(currencyName);
        if (sources.Count > 0)
        {
            sb.AppendLine(GetString($"\n可以用以下货币兑换 {currencyName}:"));
            foreach (var path in sources)
            {
                sb.AppendLine(GetString($"  ← {path.FromCurrency} (1:{path.Rate})"));
            }
        }
        else
        {
            sb.AppendLine(GetString($"\n{currencyName} 无法通过兑换获得（只能通过游戏行为获得）"));
        }

        return sb.ToString();
    }

    public string GetCurrencyGraph()
    {
        var sb = new StringBuilder();
        sb.AppendLine(GetString("=== 货币兑换关系图 ==="));

        foreach (var currency in Setting.Instance.Currencies.OrderBy(c => c.Name))
        {
            sb.AppendLine(GetString($"\n[{currency.Name}]"));
            if (!string.IsNullOrEmpty(currency.Description))
            {
                sb.AppendLine(GetString($"  描述: {currency.Description}"));
            }

            if (currency.CanExchangeToOthers)
            {
                sb.AppendLine(GetString("  可兑换为:"));
                foreach (var (targetName, rate) in currency.ExchangeRates)
                {
                    sb.AppendLine(GetString($"    → {targetName} (1:{rate})"));
                }
            }
            else
            {
                sb.AppendLine(GetString("  无法兑换为其他货币"));
            }

            var sources = GetExchangePathsTo(currency.Name);
            if (sources.Count > 0)
            {
                sb.AppendLine(GetString("  可由以下货币兑换:"));
                foreach (var path in sources)
                {
                    sb.AppendLine(GetString($"    ← {path.FromCurrency} (1:{path.Rate})"));
                }
            }
            else
            {
                sb.AppendLine(GetString("  只能通过游戏行为获得"));
            }
        }

        return sb.ToString();
    }

    public IReadOnlyList<ArbitrageCycle> GetDetectedCycles()
    {
        return this._detectedCycles;
    }

    public Result<CurrencyDefinition> GetCurrency(string name)
    {
        var currency = Setting.Instance.GetCurrency(name);
        return currency == null
            ? Result.Failure<CurrencyDefinition>(GetString($"货币 '{name}' 不存在"))
            : Result.Success(currency);
    }

    public IReadOnlyCollection<CurrencyDefinition> GetAllCurrencies()
    {
        return Setting.Instance.Currencies;
    }

    private static List<ExchangePath> GetExchangePathsTo(string toCurrency)
    {
        var paths = new List<ExchangePath>();
        foreach (var currency in Setting.Instance.Currencies)
        {
            if (currency.ExchangeRates.TryGetValue(toCurrency, out var rate))
            {
                paths.Add(new ExchangePath
                {
                    FromCurrency = currency.Name,
                    ToCurrency = toCurrency,
                    Rate = rate
                });
            }
        }
        return paths;
    }
}
