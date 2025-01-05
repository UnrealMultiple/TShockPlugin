# Autoclear

- 作者: 大豆子[Mute适配1447]，肝帝熙恩更新
- Source: TShock Official QQ Group
- Start the cleaning countdown when the number of items on the ground reaches a certain value.
- 可自定义哪些/哪类物品不被清扫

## 指令

```
暂无
```

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

## 更新日志

```
v1.0.7
卸载函数
v1.0.6
准备更新TS 5.2.1,修正文档，初始配置内容更改
v1.0.4
添加英文翻译
```

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
