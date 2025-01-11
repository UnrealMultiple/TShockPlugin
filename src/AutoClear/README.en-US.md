# Autoclear

- Authors: 大豆子,Mute,肝帝熙恩
- Source: TShock Official QQ Group
- Start the cleaning countdown when the number of items on the ground reaches a certain value.
- Customize which items or categories of items are not to be cleaned.

## Config
> Configuration file location：tshock/Autoclear.en-US.json
```json5
{
  "Interval": 10, // How often to check (seconds)
  "Exclude": [], // List of item IDs not to be cleaned
  "Threshold": 100, // Threshold value for smart cleaning
  "Dealy": 10, // Delay cleaning (seconds)
  "DealyMsg": "", // Custom message for delayed cleaning
  "SweepSwinging": true, // Whether to clean Melee Weapons
  "SweepThrowable": true, // Whether to clean Consumables
  "SweepRegaular": true, // Whether to clean Common Items
  "SweepEquipment": true, // Whether to clean Armors
  "SweepVanity": true, // Whether to clean Vanity
  "SweepMsg": "", // Custom message for completed cleaning
  "SweepTip": true // Broadcast clean message
}

```

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
