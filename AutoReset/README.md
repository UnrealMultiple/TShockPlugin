# AutoReset 完全自动重置插件

- 作者: cc04 & Leader & 棱镜 & Cai & 肝帝熙恩
- 完全自动重置插件,自定义要重置什么
  

## 更新日志

```
v.2024.8.24
尝试完善卸载函数
```

## 指令

| 语法           |        权限         |   说明   |
| -------------- | :-----------------: | :------: |
| /reset 或 /重置世界	     | reset.admin      |  重置世界|
| /rs 或 /重置设置	     | reset.admin      |  重置设置|


## 文件位置
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

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/Controllerdestiny/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
