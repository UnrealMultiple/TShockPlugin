# AutoReset fully automatic reset plug-in

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: cc04 & Leader & Prism & Cai&Gan Di Xi En
- Fully automatically reset the plug-in, customize what to reset.
  

## Update log

```
v.2024.8.24
尝试完善卸载函数
```

## instruction

|grammar|limit of authority|explain|
| -------------- |:-----------------:|:------:|
|/reset or/reset the world|reset.admin|Reset the world|
|/rs or/reset settings|reset.admin|Reset settings|


## File location
> tshock/AutoReset/reset_config.json
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
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.