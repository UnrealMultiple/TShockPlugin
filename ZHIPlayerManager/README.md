# ZHIPlayerManager zhi的玩家管理插件
- 作者: z枳，羽学，肝帝熙恩进行修改
- 出处: [github](https://github.com/skywhale-zhi/ZHIPlayerManager)
- 玩家管理，提供修改玩家的任何信息，允许玩家备份，可以回档等操作

 ## 更新日志

```
v.1.0.0.5
为是否允许特殊名字加了个总开关
v.1.0.0.4
修复mysql没有指定键长度,修改中文变量为英文，为缺少权限的指令重新添加权限
```


## 功能介绍

### 1. 查看玩家库存和状态
查背包的功能，能查看1.44更新的额外背包栏，无论是否离线。查状态的功能，目前能显示生命值，魔力值，钓鱼任务数目，永久增益，Buff数目，钱币数目，额外数据还有：在线时长，击杀生物数，击杀Boss情况，击杀罕见生物数，死亡次数等等，支持离线

### 2. 修改玩家信息
允许修改玩家几乎任何信息，包括，生命、生命上限、魔力、魔力上限、渔夫任务数、火把神激活、恶魔心激活、工匠面包、生命水晶、埃癸斯果、奥术水晶、银河珍珠、粘性蠕虫、珍馐、超级矿车的修改，支持修改在线和不在线玩家

### 3. 允许玩家备份和自动备份
玩家有时候会在服务器里丢失存档，尤其在游戏卡顿的时候，该插件允许玩家自己手动备份存档或自动备份，最多五个存档栏超过五个最旧的会自动删除（默认5个，可以调）， **回档权限不要给玩家** ，如果这样玩家就可以自由刷东西，建议让管理持有回档权限， **数据记录在 tshock.sqlite 的 Zhipm_PlayerBackUp 表中** 。无论玩家是否在线、玩家的默认装备栏和备份的默认装备栏是否一致都能成功，下面的复制人物存档指令也是一样，相当便捷

### 4. 清理玩家数据
该插件有多种清理类型的指令，详情输入`/zreset help`查看。而在服务器开荒时输入指令`/zresetallplayers`则可以直接清理所有玩家的所有数据

### 5. 复制玩家存档
你可以随时把一个人的存档复制给另一个人，无论他们是否在线

### 6. 记录玩家游玩时间
本插件会记录玩家游玩的时间，不会随着关服而消失， **数据记录在 tshock.sqlite 的 Zhipm_PlayerExtra 表中**，可以选择是否启用

### 7. 数据排行榜
目前有游玩时间排榜，总钱数排榜，完成任务鱼数排榜，击杀生物数排行榜，击杀稀有生物数排行榜，击杀Boss数排行榜，在 `/zsort` 系列指令中查看

### 8. 更友好的ban指令
此ban指令 `/zban` 支持离线ban，同时封禁 acc，ip，uuid，对名字支持模糊搜索，但只会在找到唯一一个玩家才会ban掉，若模糊搜索找到不止一个玩家，为防止误ban，该指令会让你重新输入

### 9. 允许导出玩家人物存档
`/zout`系列指令允许导出人物数据，并按照当前地图名称为文件夹打包，除了人物数据缺失会导致无法导出，其他都能导出

### 10. 冻结玩家
`/zfre name`指令能直接冻结玩家，从acc, ip, uuid 三个数据进行比对，若符合则直接冻结。可以冻结离线玩家，当他们再次进入服务器时起效。注意，此功能在服务器重启后失效，仅用于临时冻结，若要长期请使用`/zban`

### 11. 清理无效服务器数据
这个功能打算以后从这个插件里移除，因为有点跑题。使用`/zclear`指令将在20秒后清理世界内所有非Boss非城镇NPC的无用NPC，清理所有掉落在地上的物品，清理所有射弹，减轻服务器内无效数据。

### 12. 统计玩家击杀Boss的伤害输出
在玩家杀死Boss该插件能统计各个玩家的输出情况并广播输出，玩家可自行观看自己的战斗贡献，甚至可以自己设置统计哪些生物，比如蓝色史莱姆

## 指令

| 功能分类 | 权限 | 指令 & 功能 |
| --- | --- | --- |
| **帮助系列** | `zhipm.help` | `/zhelp`: 查看该插件下的所有指令帮助 |
| **保存系列** | `zhipm.save` | 
|  |  | `/zsave`: 备份自己的人物存档 |
|  |  | `/zvisa [num]`: 查看自己备份的库存，`num`范围 1~5（默认5，可改） |
|  |  | `/zvisa [name] [num]`: 查看某人备份的库存，`num`范围 1~5 |
|  |  | `/zsaveauto [num]`: 允许用户自动备份，每隔`num`分钟一次 |
| **回档系列** | `zhipm.back` | `/zback [name] [num]`: 让玩家回档到`num`号备份 |
| **复制系列** | `zhipm.clone` | `/zclone [name1] [name2]`: 将`name1`的数据复制给`name2` |
| **修改系列** | `zhipm.modify` | 
|  |  | `/zmodify help`: 查看`zmodify`系列指令帮助 |
|  |  | `/zmodify [name] life [num]`: 修改生命值 |
|  |  | `/zmodify [name] lifemax [num]`: 修改生命上限 |
|  |  | `/zmodify [name] mana [num]`: 修改魔力值 |
|  |  | `/zmodify [name] manamax [num]`: 修改最大魔力值 |
|  |  | `/zmodify [name] fish [num]`: 修改钓鱼完成数 |
|  |  | `/zmodify [name] torch [0或1]`: 开启或关闭火把神增益 |
|  |  | ... (省略其他增益指令) |
|  |  | `/zmodify [name] point [num]`: 修改点数 |
| **冻结系列** | `zhipm.freeze` | 
|  |  | `/zfre [name]`: 冻结玩家 |
|  |  | `/zunfre [name]`: 解冻玩家 |
|  |  | `/zunfre all`: 解冻所有玩家 |
| **重置系列** | `zhipm.reset` | 
|  |  | `/zresetdb [name]`: 重置备份数据 |
|  |  | `/zresetdb all`: 重置所有备份数据 |
|  |  | `/zresetex [name]`: 重置额外数据 |
|  |  | `/zresetex all`: 重置所有额外数据 |
|  |  | `/zreset [name]`: 重置人物数据 |
|  |  | `/zreset all`: 重置所有人物数据 |
|  |  | `/zresetallplayers`: 重置所有玩家所有数据 |
| **查背包系列** | `zhipm.vi` | 
|  |  | `/vi [name]`: 查询玩家库存(按顺序) |
|  |  | `/vid [name]`: 查询玩家库存(不按顺序) |
| **查状态信息系列** | `zhipm.vs` | 
|  |  | `/vs [name]`: 查询状态数据 |
|  |  | `/vs me`: 查询自己状态 |
| **排榜系列** | `zhipm.sort` | 
|  |  | `/zsort help`: 查看排榜指令帮助 |
|  |  | `/zsort time [num/all]`: 排序在线时间 |
|  |  | `/zsort coin [num/all]`: 排序钱币数 |
|  |  | ... (省略其他排序指令) |
|  |  | `/zsort clumsy [num/all]`: 排序菜鸡值 |
| **导出数据系列** | `zhipm.out` | `/zout [name/all]`: 导出玩家存档 |
| **超级ban系列** | `zhipm.ban` | `/zban add [name] [原因]`: 优化过的ban指令 |
| **玩家游戏体验设置** | `zhipm.hide` | `/zhide kill 或 point`: 隐藏击杀或点数提示 |
| **清理系列** | `zhipm.clear` | 
|  |  | `/zclear useless`: 清理无用物品 |
|  |  | `/zclear buff [name]`: 清除玩家所有buff |
## 配置
> 配置文件位置：tshock/Zhipm/ZhiPlayerManager.json
```json
{
  "是否启用在线时长统计": true,		//启用这个功能将记录玩家在线时长
  "是否启用死亡次数统计": true        //同上
  "是否启用击杀NPC统计": true,		//同上
  "是否启用点数统计": false,			//击杀怪物获得点数，目前处于测试，默认关闭，需 "是否启用击杀NPC统计" 开启
  "默认击杀字体是否对玩家显示": true,	//是否启用 kill + 1 的怪物击杀字体，需 "是否启用击杀NPC统计" 开启
  "默认点数字体是否对玩家显示": true,	//对应点数的字体，目前处于测试，默认关闭，需 "是否启用点数统计" 开启
  "是否启用击杀Boss伤害排行榜": true, //杀死Boss时统计并发送玩家的伤害贡献，需 "是否启用击杀NPC统计" 开启
  "是否启用玩家自动备份": false,		//自动备份，区别于手动备份
  "默认自动备份的时间_单位分钟_若为0代表关闭": 20,  //每隔 20 分钟对服务器内在线玩家进行备份
  "每个玩家最多几个备份存档": 5,      //每个玩家最多几个备份存档
  "哪些生物也包含进击杀伤害排行榜":[]  //与击杀boss伤害排行榜对应的击杀生物伤害排行榜，可以在这里填入生物ID，需 "是否启用击杀NPC统计" 开启
  "哪些生物也当成稀有生物进行击杀记录": [],
  "是否允许玩家回溯上次死亡点": true,
  "每次死亡回溯消耗点数": 40,
  "死亡时丢失点数乘数": 1.0,
  "是否允许特殊名字": false           //例如纯数字名字
}
```

## 其他
-  **不要把备份权限和回档权限同时给玩家** ，这样做他们就可以自由刷物品了
- 该插件本质是对玩家的全方位管理集合，体积大是因为指令占比非常多，并不会占用太多服务器算力
- 该插件在tshock.sqlite里增加了两个表，Zhipm_PlayerBackUp 和 Zhipm_PlayerExtra，前者表是对 tsCharater 表的备份，主键是 AccAndSlot 由玩家账户ID和备份槽ID组成的 xxx-x 的字符串，备份槽ID 1 ~ 5，可以在配置文件中修改
- 后者表记录着本插件统计的信息: time 在线时间单位秒，backuptime 自动备份时间单位分钟，killNPCnum 击杀NPC数目，killBossID 击杀BossID和数目的组合，如4~10就是击杀克苏鲁之眼10次，killRareNPCID 击杀罕见NPC的ID和数目的组合，point 点数，hideKillTips 是否隐藏kill+1的白色悬浮字，hidePointTips 是否隐藏+1$的粉色悬浮字
- 点数是测试功能，相当于货币，优点是避免了泰拉自带的网卡狂刷钱的bug和怪物捡钱的bug(使得钱迅速贬值)，点数可以通过杀怪获得，目的是打算以后以此插件为前置插件用统计信息如点数来实现商品购买等功能，目前默认禁用，你可以启用他
- 看着指令权限挺多，其实只建议给默认玩家`zhipm.save`, `zhipm.sort`, `zhipm.vi`, `zhipm.vs` 这几个权限就够了，其他的指令超管能自动获取并使用，在游戏内可以用`/zhelp`查阅所有指令的用法，不必牢记所有指令
- **该插件对玩家名称做了点简单的限制：名字不可以为纯数字，名字不可以完全等于服务器的一些指令，名字的第一个字符不可以是特殊符号除了[**


## 已统计好的数据
- 如果你想利用此插件已整理好的数据，作为前置插件使用请参考，我建议你去看源码。这里简单介绍下
```
long Timer;  计时器，记录着服务器运行的时间，单位 1/60 秒
List<MessPlayer> frePlayers;  被冻结的玩家的集合
List<ExtraData> edPlayers { get; set; }  所有玩家数据整合的部分
public class ExtraData
{
    /// 账户ID
    int Account;
    /// 名字
    string Name;
    /// 在线总时长，单位秒
    long time;
    /// 备份间隔，单位分钟
    int backuptime;
    /// 击杀生物数
    int killNPCnum;
    /// 击杀boss的id统计，id -> 击杀数
    Dictionary<int,int> killBossID;
    /// 击杀罕见生物的id统计，id -> 击杀数
    Dictionary<int,int> killRareNPCID;
    /// 点数（一个测试功能，相当于货币）
    long point;
    /// 是否隐藏击杀 kill + 1 的字
    bool hideKillTips;
    /// 是否隐藏点数 + 1 $ 的字
    bool hidePointTips;
    ///死亡次数
    int deathCount;
}
```

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/Controllerdestiny/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love