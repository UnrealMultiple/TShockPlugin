using Economics.Core.ConfigFiles;
using Economics.Core.DB;
using Economics.Core.Model;

namespace Economics.Core.Services;

internal class CurrencyService(CurrencyManager currencyManager) : ICurrencyService
{
    private readonly CurrencyManager _currencyManager = currencyManager ?? throw new ArgumentNullException(nameof(currencyManager));

    public Result<long> GetBalance(string playerName, string currencyName)
    {
        if (string.IsNullOrWhiteSpace(playerName))
        {
            return Result.Failure<long>(GetString("玩家名称不能为空"));
        }

        if (string.IsNullOrWhiteSpace(currencyName))
        {
            return Result.Failure<long>(GetString("货币名称不能为空"));
        }

        var currency = this._currencyManager.GetUserCurrency(playerName, currencyName);
        return Result.Success(currency.Number);
    }

    public Result<Dictionary<string, long>> GetAllBalances(string playerName)
    {
        if (string.IsNullOrWhiteSpace(playerName))
        {
            return Result.Failure<Dictionary<string, long>>(GetString("玩家名称不能为空"));
        }

        var balances = this._currencyManager.GetPlayerCurrencies(playerName)
            .ToDictionary(c => c.CurrencyType, c => c.Number);

        return Result.Success(balances);
    }

    public Result AddCurrency(string playerName, string currencyName, long amount)
    {
        var validation = ValidateAmount(amount);
        if (validation.IsFailure)
        {
            return validation;
        }

        this._currencyManager.AddUserCurrency(playerName, amount, currencyName);
        return Result.Success();
    }

    public Result DeductCurrency(string playerName, string currencyName, long amount)
    {
        var validation = ValidateAmount(amount);
        if (validation.IsFailure)
        {
            return validation;
        }

        var success = this._currencyManager.DeductUserCurrency(playerName, amount, currencyName);
        if (!success)
        {
            var currentBalance = this._currencyManager.GetUserCurrency(playerName, currencyName).Number;
            return Result.Failure(GetString($"余额不足: 需要 {amount}, 拥有 {currentBalance}"));
        }

        return Result.Success();
    }

    public Result Transfer(string fromPlayer, string toPlayer, string currencyName, long amount)
    {
        if (string.IsNullOrWhiteSpace(fromPlayer))
        {
            return Result.Failure(GetString("转出玩家名称不能为空"));
        }

        if (string.IsNullOrWhiteSpace(toPlayer))
        {
            return Result.Failure(GetString("转入玩家名称不能为空"));
        }

        if (fromPlayer == toPlayer)
        {
            return Result.Failure(GetString("不能给自己转账"));
        }

        var validation = ValidateAmount(amount);
        if (validation.IsFailure)
        {
            return validation;
        }

        var deductResult = this.DeductCurrency(fromPlayer, currencyName, amount);
        if (deductResult.IsFailure)
        {
            return Result.Failure(GetString($"转账失败: {deductResult.Error}"));
        }
        this._currencyManager.AddUserCurrency(toPlayer, amount, currencyName);
        return Result.Success();
    }

    public Result ClearAllCurrencies(string playerName)
    {
        if (string.IsNullOrWhiteSpace(playerName))
        {
            return Result.Failure(GetString("玩家名称不能为空"));
        }

        foreach (var currency in Setting.Instance.Currencies)
        {
            this._currencyManager.ClearUserCurrency(playerName, currency.Name);
        }

        return Result.Success();
    }

    public List<PlayerCurrencyInfo> GetAllCurrencyRecords()
    {
        return this._currencyManager.GetCurrencies();
    }

    public PlayerCurrencyInfo[] GetPlayerCurrencyRecords(string playerName)
    {
        return this._currencyManager.GetPlayerCurrencies(playerName);
    }

    public Result DeductMultipleCurrencies(string playerName, IEnumerable<RedemptionRelationshipsOption> options, long multiplier = 1)
    {
        if (string.IsNullOrWhiteSpace(playerName))
        {
            return Result.Failure(GetString("玩家名称不能为空"));
        }

        var optionsList = options.ToList();
        if (optionsList.Count == 0)
        {
            return Result.Failure(GetString("没有指定要扣除的货币"));
        }

        foreach (var option in optionsList)
        {
            var required = option.Number * multiplier;
            var balanceResult = this.GetBalance(playerName, option.CurrencyType);
            if (balanceResult.IsFailure)
            {
                return Result.Failure(GetString($"无法查询 {option.CurrencyType} 余额"));
            }

            if (balanceResult.Value < required)
            {
                return Result.Failure(GetString($"{option.CurrencyType} 不足: 需要 {required}, 拥有 {balanceResult.Value}"));
            }
        }

        foreach (var option in optionsList)
        {
            var result = this.DeductCurrency(playerName, option.CurrencyType, option.Number * multiplier);
            if (result.IsFailure)
            {
                return Result.Failure(GetString($"扣除 {option.CurrencyType} 失败: {result.Error}"));
            }
        }

        return Result.Success();
    }

    public Result AddMultipleCurrencies(string playerName, IEnumerable<RedemptionRelationshipsOption> options, long multiplier = 1)
    {
        if (string.IsNullOrWhiteSpace(playerName))
        {
            return Result.Failure(GetString("玩家名称不能为空"));
        }

        foreach (var option in options)
        {
            var result = this.AddCurrency(playerName, option.CurrencyType, option.Number * multiplier);
            if (result.IsFailure)
                return Result.Failure(GetString($"增加 {option.CurrencyType} 失败: {result.Error}"));
        }

        return Result.Success();
    }

    public Result ResetAllCurrencies()
    {
        this._currencyManager.Reset();
        return Result.Success();
    }

    private static Result ValidateAmount(long amount)
    {
        return amount <= 0 ? Result.Failure(GetString("金额必须大于0")) : Result.Success();
    }
}
