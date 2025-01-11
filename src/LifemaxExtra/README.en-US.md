# LifemaxExtra

- Authors: 佚名，肝帝熙恩，少司命
- Source: TShock Official Group Chat
- Allows customization of items to increase health, breaking through the health limit.
- Due to the special nature of mana, the actual maximum increase can only be up to 400.
- Only usable when SSC (Forced Survival Mode) is enabled.

## Commands

| Syntax                         |    Permission    |       Description        |
|--------------------------------|:----------------:|:------------------------:|
| /hp enh [player name] [health] | lifemaxextra.use | Increase player's health |
| /hp set [player name] [health] | lifemaxextra.use |   Set player's health    |
| /hp enh [health]               | lifemaxextra.use |   Increase own health    |
| /hp set [health]               | lifemaxextra.use |      Set own health      |
| /mp enh [player name] [mana]   | lifemaxextra.use |  Increase player's mana  |
| /mp set [player name] [mana]   | lifemaxextra.use |    Set player's mana     |
| /mp enh [mana]                 | lifemaxextra.use |    Increase own mana     |
| /mp set [mana]                 | lifemaxextra.use |       Set own mana       |


## Config
> Configuration file location：tshock/LifemaxExtra.en-US.json
```json5
{
  "MaxHP": 1000,
  "MaxMP": 400,
  "ItemRaiseHP": {
    "29": {
      "Max": 600,
      "Raise": 20
    },
    "1291": {
      "Max": 100,
      "Raise": 5
    }
  },
  "ItemRaiseMP": {
    "109": {
      "Max": 400,
      "Raise": 20
    }
  }
}
```

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
