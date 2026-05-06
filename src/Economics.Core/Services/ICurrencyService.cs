using Economics.Core.ConfigFiles;
using Economics.Core.Model;

namespace Economics.Core.Services;

public interface ICurrencyService
{
    Result<long> GetBalance(string playerName, string currencyName);

    Result<Dictionary<string, long>> GetAllBalances(string playerName);

    Result AddCurrency(string playerName, string currencyName, long amount);

    Result DeductCurrency(string playerName, string currencyName, long amount);

    Result Transfer(string fromPlayer, string toPlayer, string currencyName, long amount);

    Result ClearAllCurrencies(string playerName);

    List<PlayerCurrencyInfo> GetAllCurrencyRecords();

    PlayerCurrencyInfo[] GetPlayerCurrencyRecords(string playerName);

    Result DeductMultipleCurrencies(string playerName, IEnumerable<RedemptionRelationshipsOption> options, long multiplier = 1);

    Result AddMultipleCurrencies(string playerName, IEnumerable<RedemptionRelationshipsOption> options, long multiplier = 1);

    Result ResetAllCurrencies();
}
