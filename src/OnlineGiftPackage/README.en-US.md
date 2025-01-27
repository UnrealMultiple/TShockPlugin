# Online Gift Package

- author: 星夜神花、羽学
- source: [github](https://gitee.com/star-night-flower/tshock-gift)
- This is a Tshock server plug-in mainly used to issue random online rewards to online players in the server.
- After adding items to the [在线礼包.json] file, the game can automatically calculate the total probability of obtaining it by sending/reloading it.

## Instruction

| Command      |        Permissions         |      Description       |
|---------|:-----------------:|:-------------:|
| /在线礼包   | OnlineGiftPackage | Shows the probability table for all items in the gift pack |
| /reload |         none         |    Automatically calculate the total probability    |

## Configuration
> Configuration file located: tshock/在线礼包.json
```json5
{
  "启用": true, //enable
  "总概率(自动更新)": 60, //Total probability (automatically updated)
  "发放间隔/秒": 1800, //Issue interval/second
  "跳过生命上限": 500, //Skip life limit
  "每次发放礼包记录后台": false, //Record background for each gift package issued
  "礼包列表": [ //Gift package list
    {
      "物品名称": "铂金币", //Item name
      "物品ID": 74, //Item ID
      "所占概率": 1, //Probability
      "物品数量": [ //Item quantity
        2,
        5
      ]
    }
  ],
    "触发序列": { //Trigger sequence
    "1": "[c/55CDFF:服主]送了你1个礼包"
  }
}
```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
