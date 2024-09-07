# Autoclear

- Authors: 大豆子,Mute,肝帝熙恩
- Source: TShock Official QQ Group
- Start the cleaning countdown when the number of items on the ground reaches a certain value.
- Customize which items or categories of items are not to be cleaned.

## Config
> Configuration file location：tshock/Autoclear.json
```json
{
  "多久检测一次(s)": 10, // How often to check (seconds)
  "不清扫的物品ID列表": [], // List of item IDs not to be cleaned
  "智能清扫数量临界值": 100, // Threshold value for smart cleaning
  "延迟清扫(s)": 10, // Delay cleaning (seconds)
  "延迟清扫自定义消息": "", // Custom message for delayed cleaning
  "是否清扫挥动武器": true, // Whether to clean Melee Weapons
  "是否清扫投掷武器": true, // Whether to clean Consumables
  "是否清扫普通物品": true, // Whether to clean Common Items
  "是否清扫装备": true, // Whether to clean Armors
  "是否清扫时装": true, // Whether to clean Vanity
  "完成清扫自定义消息": "", // Custom message for completed cleaning
  "具体消息": true // Broadcast clean message
}
```

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
