# Economicsapi plug -in [Economic Kit Pre -in front]

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Shao Si Ming
- Source: None
- Economic kit front plug -in

## Update log

```
V2.0.0.0
- 添加扩展函数
- 添加显示消息API
- 添加渐变色消息API
- 自定义渐变色消息颜色
- 修复死亡掉落货币
- 修复玩家伤害计算失准
```

## instruction

|grammar|Authority|illustrate|
| --------------------------- |: -----------------------:|: -------:|
|/Bank add [Player Name] [Number]|economics.bank|Add currency|
|/Bank Del [Player Name] [Number]|economics.bank|Delete currency|
|/Bank Pay [Player Name] [Number]|economics.bank.pay|Transfer currency|
|/Query|economics.currency.query|Query currency|

## Configuration
> Configuration file location: TSHOCK/ECONOMICS/Economics.json
```json
{
   "货币名称": "魂力",
   "货币转换率": 1.0,
   "保存时间间隔": 30,
   "显示收益": true,
   "禁用雕像": false,
   "死亡掉落率": 0.0,
   "显示信息": true,
   "显示信息左移": 60,
   "显示信息下移": 0,
   "查询提示": "[c/FFA500:你当前拥有{0}{1}个]",
   "渐变颜色": [
     "[c/00ffbf:{0}]",
     "[c/1aecb8:{0}]",
     "[c/33d9b1:{0}]",
     "[c/80a09c:{0}]",
     "[c/998c95:{0}]",
     "[c/b3798e:{0}]",
     "[c/cc6687:{0}]",
     "[c/e65380:{0}]",
     "[c/ff4079:{0}]",
     "[c/ed4086:{0}]",
     "[c/db4094:{0}]",
     "[c/9440c9:{0}]",
     "[c/8240d7:{0}]",
     "[c/7040e4:{0}]",
     "[c/5e40f2:{0}]",
     "[c/944eaa:{0}]",
     "[c/b75680:{0}]",
     "[c/db5d55:{0}]",
     "[c/ed6040:{0}]",
     "[c/ff642b:{0}]" 
  ]
}
```
## feedback
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love