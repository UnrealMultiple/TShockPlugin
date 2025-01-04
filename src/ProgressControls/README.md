# ProgressControl 计划书

- 作者: 枳、羽学
- 出处: [github](https://github.com/skywhale-zhi/ProgressControl)
- 这是一个Tshock服务器插件主要用于自动化：
- 重置服务器、重启服务器、执行命令、控制NPC进度、备份或删除文件
- 1.能够自动重置地图
- 可设置重置地图的大小、模式、种子、名称、地图存放目录、等功能
- 2.能自动重启地图
- 可按照当前的端口，地图数据来重启泰拉瑞亚服务器
- 3.能自动执行指令
- 自动执行你要设定的指令，支持原版和其他插件的
- 4.按时间来对Boss和NPC进行封禁，时间未到不可打，会自动消失
- 5.指令功能还可以删除或复制指定路径文件(进行清理与备份)


## 指令

| 语法               |    别名     |     权限     |               说明                |
|------------------|:---------:|:----------:|:-------------------------------:|
| /pco help        |   /pco    |     无      |         查看这个插件下的所有帮助指令          |
| /pco now         |     无     | pco.admin  | 将现在时间同步为：开服日期、上次重启日期和上次自动执行指令日期 |
| /pco delFile     |     无     | pco.admin  |        删除配置项中指定路径文件或文件夹         |
| /pco copy        |     无     | pco.admin  |      复制配置项中指定路径文件或者文件夹并重命名      |
| /pco npc help    |     无     |  pco.npc   |         查看控制NPC进度的指令帮助          |
| /pco com help    |     无     |  pco.com   |           查看执行指令的指令帮助           |
| /pco reload help |     无     | pco.reload |           查看自动重启的指令帮助           |
| /pco reset help  |     无     | pco.reset  |           查看自动重置的指令帮助           |
| /pco mess        | /pco view | 拥有以上任意1个权限 |       来查看当前服务器的自动化计划，详细版        |



## 配置
> 配置文件位置：tshock/ProgressControl.json
```json5
{
  "开服日期": "2024-06-14T17:15:20.1069407+08:00",
  "是否启用自动重置世界": false,
  "多少小时后开始自动重置世界": 176.0,
  "重置是否重置玩家数据": true,
  "重置前是否删除地图": true,
  "NPC死亡次数触发执行指令": {
    "398": [
      3,
      "pco reset hand",
      "bc 击杀月总3次自动重置"
    ]
  },
  "重置前执行的指令": [
    "clall",
    "zout all",
    "wat clearall",
    "pbreload",
    "pco copy",
    "礼包 重置",
    "礼包重置",
    "pvp reset",
    "派系 reset",
    "bwl reload",
    "task clear",
    "task reset",
    "rpg reset",
    "bank reset",
    "deal reset",
    "skill reset",
    "level reset",
    "replenreload",
    "重读多重限制",
    "重读阶段库存",
    "clearbuffs all",
    "重读物品超数量封禁",
    "重读自定义怪物血量",
    "重读禁止召唤怪物表",
    "zresetallplayers",
    "clearallplayersplus",
    "reload"
  ],
  "重置前删除哪些数据库表": [
    "HousingDistrict",
    "TerrariaRobot死亡统计",
    "Warps",
    "渡劫表",
    "RememberedPos",
    "Zhipm_PlayerBackUp",
    "Zhipm_PlayerExtra",
    "Research",
    "使用日志",
    "区域执行指令",
    "Economics",
    "Economicsskill",
    "Regions",
    "RPG",
    "Skill",
    "Permabuff",
    "Permabuffs",
    "Onlineduration",
    "Onlybaniplist",
    "Stronger",
    "Synctable",
    "Task",
    "TaskKillnpc",
    "TaskTallk",
    "OnlineDuration",
    "WeaponPlusDBcostCoin",
    "WeaponPlusdbbasedOnEconomics"
  ],
  "重置前删除哪些文件或文件夹": [
    "tshock/backups/*.bak",
    "tshock/logs/*.log",
    "tshock/Watcher/logs/*.log",
    "tshock/检查背包/检查日志/*.txt"
  ],
  "重置后的地图大小_小1_中2_大3": 3,
  "重置后的地图难度_普通0_专家1_大师2_旅途3": 2,
  "重置后的地图种子": "",
  "重置后的地图名称": "SFE4",
  "你提供用于重置的地图名称": [],
  "地图存放目录": "./world/",
  "重置后的最多在线人数": 32,
  "重置后的端口": "7777",
  "重置后的服务器密码": "",
  "上次重启服务器的日期": "2024-06-14T17:15:20.1069667+08:00",
  "是否启用自动重启服务器": false,
  "多少小时后开始自动重启服务器": 0.0,
  "重启后的最多在线人数": 32,
  "重启后的端口": "7777",
  "重启后的服务器密码": "",
  "重启前执行的指令": [],
  "是否自动控制NPC进度": false,
  "Boss封禁时长距开服日期": {
    "史莱姆王": 0.0,
    "克苏鲁之眼": 0.0,
    "世界吞噬者": 24.0,
    "克苏鲁之脑": 24.0,
    "蜂后": 42.0,
    "巨鹿": 36.0,
    "骷髅王": 48.0,
    "血肉墙": 72.0,
    "史莱姆皇后": 84.0,
    "双子魔眼": 96.0,
    "毁灭者": 102.0,
    "机械骷髅王": 108.0,
    "猪龙鱼公爵": 120.0,
    "世纪之花": 132.0,
    "光之女皇": 138.0,
    "石巨人": 150.0,
    "拜月教教徒": 162.0,
    "四柱": 164.0,
    "月亮领主": 170.0
  },
  "NPC封禁时长距开服日期_ID和单位小时": {},
  "上次自动执行指令的日期": "2024-06-14T17:15:20.1069949+08:00",
  "是否启用自动执行指令": false,
  "多少小时后开始自动执行指令": 0.0,
  "自动执行的指令_不需要加斜杠": [],
  "执行指令时是否发广播": true,
  "越权检查": true,
  "是否关闭ServerLog写入功能(Windows千万别开)": false,
  "指令功能_删除哪些文件或文件夹": [],
  "指令功能_要复制的文件或文件夹": [
    "world/SFE4.wld"
  ],
  "指令功能_复制目标目录": "tshock/Zhipm/",
  "指令功能_文件是否允许覆盖": true
}
```

## 更新日志

```
更新日志
v1.0.8
完善卸载函数

v1.0.7
修复新建配置没有内置参数问题

1.0.6 
修复/reload导致配置文件被覆盖问题

1.0.5
修复了Windows系统使用Bat脚本中goto start循环启动服务器时无法重置的问题
加入了/pco copy指令复制指定路径文件夹，并重命名源文件加上日期

1.0.4
适配了Linux系统与面板服的自动重置，
修复了用Bash脚本中while true;done循环启动服务器时无法重置的问题

1.0.3
加入了重置时删除指定文件，
加入了/pco delfile指令删除指定路径文件夹
根据击杀NPCID记数执行命令

```

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
