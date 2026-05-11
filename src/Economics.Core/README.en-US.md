# Economics.Core Plugin [Economic Suite Core]

- Author: Shao Siming, Qian Yi (Bug Fixes)
- Source: None
- Economic Suite Core Plugin

## Commands

| Syntax                                    |        Permission        |                           Description                           |
|-------------------------------------------|:------------------------:|:---------------------------------------------------------------:|
| /bank add <player name> <amount> <currency> |      economics.bank      |                        Increase currency                        |
| /bank deduct <player name> <amount> <currency> |      economics.bank      |                         Deduct currency                         |
| /bank pay <player name> <amount> <currency> |    economics.bank.pay    |                        Transfer currency                        |
| /bank query [player name]                 |   economics.bank.query   |                         Query currency                          |
| /bank clear <player name>                 |      economics.bank      |                         Clear currency                          |
| /bank reset                               |      economics.bank      |                      Global currency reset                      |
| /bank exchange <source> <target> <amount> |   economics.bank.cash    |                     Exchange currency                           |
| /bank preview <source> <target> <amount>  |   economics.bank.cash    |                Preview exchange result                          |
| /bank rates [currency]                    |   economics.bank.query   |                   View exchange rates                           |
| /bank cycles                              |   economics.bank.admin   |              View detected arbitrage cycles                     |
| /query                                    | economics.currency.query | Query currency (Deprecated, will be removed in future versions) |

## Configuration
> Configuration file location: tshock/Economics/Economics.json
```json5
{
  "SaveTimeInterval": 30,
  "ShowAboveHead": true,
  "IgnoreStatue": false,
  "StatusText": true,
  "StatusTextShiftLeft": 60,
  "StatusTextShiftDown": 0,
  "GradientColor": [
    "[c/00ffbf:{0}]",
    "[c/1aecb8:{0}]",
    "[c/33d9b1:{0}]",
    "[c/A6D5EA:{0}]",
    "[c/A6BBEA:{0}]",
    "[c/B7A6EA:{0}]",
    "[c/A6EAB3:{0}]",
    "[c/D5F0AA:{0}]",
    "[c/F5F7AF:{0}]",
    "[c/F8ECB0:{0}]",
    "[c/F8DEB0:{0}]",
    "[c/F8D0B0:{0}]",
    "[c/F8B6B0:{0}]",
    "[c/EFA9C6:{0}]",
    "[c/00ffbf:{0}]",
    "[c/1aecb8:{0}]"
  ],
  "Currencies": [
    {
      "QueryFormat": "[c/FFA500: You have {0}{1}]",
      "Name": "Soul Power",
      "ExchangeRates": {
        // Exchange rates: 1 this currency = ? target currency
        // Example: "Gold": 100 means 1 Soul Power = 100 Gold
      },
      "CurrencyObtain": {
        "CurrencyObtainType": 1, // 0: None, 1: Kill NPC, 2: Mine Tile
        "GiveCurrency": 0,
        "ConversionRate": 1.0,
        "ContainsID": [50] // Tile or NPC IDs
      },
      "DeathFallOption": {
        "Enable": false,
        "DropRate": 0.1
      },
      "CombatMsgOption": {
        "Enable": false,
        "CombatMsg": "+{0}$",
        "Color": [255, 255, 255]
      }
    }
  ]
}
```

## API for Other Plugins

Other plugins can access the economic system through the following APIs:

```csharp
// Query balance
var balanceResult = Core.Economics.CurrencyService.GetBalance("PlayerName", "Gold");
if (balanceResult.IsSuccess)
{
    long balance = balanceResult.Value;
}

// Add currency
Core.Economics.CurrencyService.AddCurrency("PlayerName", "Gold", 100);

// Deduct currency
var result = Core.Economics.CurrencyService.DeductCurrency("PlayerName", "Gold", 50);
if (result.IsSuccess) { /* Success */ }

// Exchange currency
var exchangeResult = Core.Economics.ExchangeService.ExecuteExchange(
    "PlayerName", "Gold", "Diamond", 10);

// Reset all currencies
var resetResult = Core.Economics.CurrencyService.ResetAllCurrencies();
```

## Changelog

### v3.0.0.0
- **Major Architecture Refactoring**: Redesigned the currency system with clearer API interfaces
- **New Currency Exchange System**: Support for exchanging between any currencies with automatic arbitrage cycle detection and prevention
- **New Service Layer**: Introduced `ICurrencyService` and `IExchangeService` interfaces for easy plugin integration
- **New Exchange Commands**:
  - `/bank exchange <source> <target> <amount>` - Execute currency exchange
  - `/bank preview <source> <target> <amount>` - Preview exchange result
  - `/bank rates [currency]` - View exchange rates
  - `/bank cycles` - View detected arbitrage cycles
- **Removed Deprecated API**: `CurrencyManager` is no longer exposed externally, please use `Economics.CurrencyService`
- **Configuration Changes**: `CustomizeCurrencys` renamed to `Currencies`, configuration structure is clearer
- **Code Optimization**: Streamlined CurrencyManager, unified use of PlayerCurrencyInfo model

### v2.1.0.0
- Fixed currency rewards based on damage output not receiving full amount when killing Bosses: Previously affected by last-hit damage loss, weapon damage vs actual HP reduction inconsistency, and DoT (poison/burn) damage not being counted; now solo kills consistently receive full rewards, multiplayer kills are distributed based on damage percentage dealt to the Boss

### v2.0.0.13
- Fixed issue where currency was calculated based on weapon raw damage when killing low HP monsters, now overflow damage is clipped to NPC's actual remaining HP

### v2.0.0.12
- Fixed /bank lb not displaying

### v2.0.0.11
- Fixed /bank query specifying player with no output issue

### v2.0.0.10
- Fixed CombatMsg not properly displaying currency gained after kills

### v2.0.0.8
- Renamed to Economics.Core
- Updated command system
- Updated configuration file system

### v2.0.0.0
- Added multi-currency implementation

### v1.0.2.0
- Added /bank query command to replace /query
- BREAKING CHANGE: CurrencyManager.DelUserCurrency renamed to CurrencyManager.DeductUserCurrency

### v1.0.0.0
- Added extension functions
- Added display message API
- Added gradient color message API
- Custom gradient color message colors
- Fixed death drop currency
- Fixed player damage calculation inaccuracy

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
