# Dummy 假人

- 作者: 少司命
- 出处: 此仓库
- 此插件提供一些假人进入你得服务器，他们本质是一名玩家。

## 说明
- 添加过多假人至服务器可能会导致服务器卡顿
- 此插件处于雏形阶段等待被扩展
- 插件有崩溃的风险


## 指令
| 语法           |        权限         |   说明   |
| -------------- | :-----------------: | :------: |
| /dummy list | dummy.client.use   | 查看假人|
| /dummy remove [index] | dummy.client.use   | 移除假人|
| /dummy reconnect [index] | dummy.client.use   | 重新连接 |

## 配置
> 配置文件位置：tshock/Dummmy.zh-CN.json
```json5
{
  "假人": [
    {
      "Type": 4, //角色类型
      "PlayerSlot": 0,  //不用填
      "SkinVariant": 0, 
      "Hair": 0, //发型
      "Name": "熙恩", //假人名称
      "HairDye": 0, 
      "Bit1": {
        "value": 0
      },
      "Bit2": {
        "value": 0
      },
      "HideMisc": 0,
      "HairColor": { //头发颜色
        "R": 239,
        "G": 211,
        "B": 211
      },
      "SkinColor": { //肤色
        "R": 239,
        "G": 202,
        "B": 202
      },
      "EyeColor": { //眼睛颜色
        "R": 51,
        "G": 9,
        "B": 9
      },
      "ShirtColor": {  //衣服颜色
        "R": 239,
        "G": 211,
        "B": 211
      },
      "UnderShirtColor": { //衣服颜色
        "R": 239,
        "G": 211,
        "B": 211
      },
      "PantsColor": { //裤子颜色
        "R": 239,
        "G": 211,
        "B": 211
      },
      "ShoeColor": { //鞋子颜色
        "R": 239,
        "G": 211,
        "B": 211
      },
      "Bit3": {
        "value": 0
      },
      "Bit4": {
        "value": 0
      },
      "Bit5": {
        "value": 0
      }
    }
  ]
}
```

## 更新日志

```
无
```

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
