# Autoreset completely automatically reset the plug -in

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: CC04 & Leader & Prism & Cai & Liver Emperor Xien
- Completely reset the plug -in, what should I customize what to reset
  

## Update log

```
v.2024.8.24
尝试完善卸载函数
```

## instruction

|grammar|Authority|illustrate|
| -------------- |: --------------------:|: -------:|
|/reset or /reset the world|reset.admin|Reset the world|
|/rs or /reset settings|reset.admin|Reset|


## File location
> TSHOCK/Autoreset/Reset_config.json
```json
{
   "替换文件": {
     "/tshock/原神.json": "原神.json",
     "/tshock/XSB数据缓存.json": ""//表示删除/tshock/XSB数据缓存.json
  },
   "击杀重置": {
     "击杀重置开关": false,
     "已击杀次数": 0,
     "生物ID": 50,
     "需要击杀次数": 50
  },
   "重置后指令": [
     "/reload",
     "/初始化进度补给箱",
     "/rpg reset" 
  ],
   "重置前指令": [
     "/结算金币" 
  ],
   "重置后SQL命令": [
     "DELETE FROM tsCharacter" 
  ],
   "地图预设": {
     "地图名": null,
     "地图种子": null
  },
   "重置提醒": false,
   "CaiBot服务器令牌": "西江超级可爱喵" 
}
```

## feedback
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love