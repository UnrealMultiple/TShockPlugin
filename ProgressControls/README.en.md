# ProgressControl plan

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Authors: Fructus Aurantii and Feather Studies
- - 出处: [github](https://github.com/skywhale-zhi/ProgressControl) 
- This is a Tshock server plug-in mainly used for automation:
- Reset the server, restart the server, execute commands, control NPC progress, back up or delete files.
- 1. The map can be automatically reset.
- You can set the functions of resetting the size, mode, seed, name, map storage directory and so on of the map.
- 2. Can automatically restart the map
- Tara rhea server can be restarted according to the current port and map data.
- 3. Can automatically execute instructions
- Automatically execute the instructions you want to set, and support the original and other plug-ins.
- 4. The Boss and NPC are banned according to the time. If you can't play before the time, it will disappear automatically.
- 5. The command function can also delete or copy files with specified paths (for cleaning and backup).

## Update log

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

## instruction

|grammar|another name|limit of authority|explain|
| -------------- |:---------:|:------------:|:------:|
|/pco help|/pco|without|View all the help instructions under this plugin.|
|/pco now|without|pco.admin|Synchronize the current time as: service opening date, last restart date and last automatic instruction execution date.|
|/pco delFile|without|pco.admin|Delete that file or folder with the specify path in the configuration item.|
|/pco copy|without|pco.admin|Copy the path file or folder specified in the configuration item and rename it.|
|/pco npc help|without|pco.npc|View the command help for controlling NPC progress.|
|/pco com help|without|pco.com|View instruction help for executing instructions.|
|/pco reload help|without|pco.reload|Check the instruction help for automatic restart.|
|/pco reset help|without|pco.reset|View the instruction help for automatic reset.|
|/pco mess|/pco view|Have any one of the above permissions|To view the automation plan of the current server, detailed version.|



## deploy
> Configuration file location: tshock/ProgressControl.json
```
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

## feedback
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.