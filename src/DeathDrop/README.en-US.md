# DeathDrop

- author: 大豆子，肝帝熙恩更新优化
- source: TShock Chinese official group
- Allows customization of what monsters drop when they die.
- Random or customized, does not affect each other

## Instruction

```
none
```

## Configuration
> Configuration file location: tshock/死亡掉落配置表.json
```json5
{
  "是否开启随机掉落": false, //Whether to turn on random drops //The main switch for random drops. This must be set to true to set content other than customization.
  "完全随机掉落": false, //Completely random drop //Completely random drop, select one or more items from 1-5452
  "完全随机掉落排除物品ID": [], //Completely random drop excluded item ID //The items here will not be selected
  "普通随机掉落物": [], //Ordinary random drops //If the completely random drops are false, you can customize the random drops of all monsters here, and the random drops are selected from here
  "随机掉落概率": 100, //Random drop probability //Probability, affecting both completely random drops and ordinary random drops
  "随机掉落数量最小值": 1, //Minimum number of random drops //Minimum number of random drops, affecting both completely random drops and ordinary random drops
  "随机掉落数量最大值": 1, //Maximum number of random drops //Minimum number of random drops, affecting both completely random drops and ordinary random drops
  "是否开启自定义掉落": false, //Whether to enable custom drops, //Custom drops are not affected by all the above settings and work independently.
  "自定义掉落设置": [ //Custom drop settings
    {
      "生物id": 0, //bioid
      "完全随机掉落": false, //Completely random drops
      "完全随机掉落排除物品ID": [], //Completely random drop excluded item ID
      "普通随机掉落物": [], //Common random drops
      "随机掉落数量最小值": 1, //Minimum random drop quantity
      "随机掉落数量最大值": 1, //Maximum number of random drops
      "掉落概率": 100 //Drop probability
    }
  ]
}
```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
