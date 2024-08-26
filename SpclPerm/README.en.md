# SPCLPERM serving the owner's privilege

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!

- Author: Yu Xue
- Source: None
- Enter the service or resurrected automatically according to the list: Invincible, BUFF, items, commands, setting backpacks, setting backpacks,
- The user group name ignores all switches, and the switch is only valid for the privilege list. The user group is needed to directly delete the group name

## Update log

```
v1.8.1
完善卸载函数

v1.8.0
把物品与BUFF表改为字典键值

v1.7.0
加入非特权背包，来区分普通用户的SSC默认背包
修复了进服给不到无敌、特权玩家进服导致其他人进服也会有特权背包问题
加入了对存在特别名单，未注册的玩家检测

v1.6.0
加入设置特权SSC背包

v1.5.0
对配置文件重新拍板

v1.4.0
移除权限名，加入对指定用户组名开启

v1.3.0
加入权限名

v1.2.0
执行命令采用临时超管权限执行，不用控制台执行

v1.1.0
BUFF支持-1永久写法

v1.0.0
根据名单进服自动无敌/给buff/物品/执行命令
```

## instruction

```
暂无
```

## Configuration
> Configuration file location: TSHOCK/Lord Privilege .json
```json
{
   "使用说明": "根据名单进服或复活自动执行：无敌、BUFF、物品、命令、设置背包，用户组名无视所有开关，开关只对特权名单有效，需关用户组权限直接删组名",
   "进服给无敌": true,
   "进服用命令": false,
   "进服给物品": false,
   "进服给BUFF": true,
   "设置SSC背包": true,
   "特权名单/受以上开关影响": [
     "羽学",
     "灵乐" 
  ],
   "所有特权的用户组名": "admin,owner",
   "无敌权限的用户组名": "admin,owner",
   "物品权限的用户组名": "admin,owner",
   "BUFF权限的用户组名": "admin,owner",
   "命令权限的用户组名": "admin,owner",
   "特权背包的用户组名": "admin,owner",
   "命令表": [
     "/clear i 9999",
     ".clear p 9999" 
  ],
   "物品表（ID:数量）": {
     "4346": 1
  },
   "Buff表（ID:分钟）": {
     "1": 10,
     "11": -1,
     "12": -1,
     "3": -1,
     "192": -1,
     "165": -1,
     "207": -1,
     "274": -1,
     "63": -1,
     "257": -1
  },
   "特权背包表": [
    {
       "初始血量": 200,
       "初始魔力": 100,
       "初始物品": [
        {
           "netID": 4956,
           "prefix": 81,
           "stack": 1
        },
        {
           "netID": 4346,
           "prefix": 0,
           "stack": 1
        },
        {
           "netID": 5437,
           "prefix": 0,
           "stack": 1
        }
      ]
    }
  ],
   "非特权背包表": [
    {
       "初始血量": 100,
       "初始魔力": 20,
       "初始物品": [
        {
           "netID": -15,
           "prefix": 0,
           "stack": 1
        },
        {
           "netID": -13,
           "prefix": 0,
           "stack": 1
        },
        {
           "netID": -16,
           "prefix": 0,
           "stack": 1
        }
      ]
    }
  ]
}
```
## feedback
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love