# ZHIPlayerManager Zhi's Player Management Plug-in

- 作者: z枳，羽学，肝帝熙恩进行修改
- Source: [github](https://github.com/skywhale-zhi/ZHIPlayerManager)
- Player management, provides the ability to modify any player information, allows players to back up, roll back, etc.

## Function introduction

### 1. View player inventory and status

查背包的功能，能查看1.44更新的额外背包栏，无论是否离线。查状态的功能，目前能显示生命值，魔力值，钓鱼任务数目，永久增益，Buff数目，钱币数目，额外数据还有：在线时长，击杀生物数，击杀Boss情况，击杀罕见生物数，死亡次数等等，支持离线

### 2. Modify player information

Allows to modify almost any player information, including life, life limit, mana power, mana power limit, number of angler quest tasks, torch god activation, demon heart activation, artisan loaf, life crystal, Aegis fruit, vital crystal, arcane crystal, galaxy pearl, gummy worms, ambrosia, and minecarts upgrade kit, supporting modifications to both online and offline players

### 3. Allow player backup and automatic backup

Players sometimes lose their save in the server, especially when the game freezes. This plug-in allows players to manually back up their save or automatically back up their save. If there are up to five save columns, the oldest one will be automatically deleted if it exceeds the five. (The default is 5, which can be adjusted, **Don’t give players the right to rollback**, otherwise players can freely grind things.It is recommended that the administrator has the rollback permission, **data is recorded in the Zhipm_PlayerBackUp table of tshock.sqlite**.It can be successful regardless of whether the player is online or not, and whether the player's default equipment column and the backup default equipment column are consistent. The following copy character save command is also the same, which is quite convenient.无论玩家是否在线、玩家的默认装备栏和备份的默认装备栏是否一致都能成功，下面的复制人物存档指令也是一样，相当便捷

### 4. Clean player data

该插件有多种清理类型的指令，详情输入`/zreset help`查看。This plug-in has various cleaning types of commands. For details, enter `/zreset help` to view. Entering the command `/zresetallplayers` during server development can directly clear all data of all players.

### 5. Copy player save

You can copy one person's save to another at any time, whether they are online or not

### 6. Record player play time

This plug-in will record the player's playing time and will not disappear when the server is closed. **The data is recorded in the Zhipm_PlayerExtra table of tshock.sqlite**. You can choose whether to enable it or not.

### 7. Data rankings

Currently, there are rankings of play time, total money, number of angler quest completed, number of mobs killed, number of rare mobs killed, and number of bosses killed, in the `/zsort` series of commands Check

### 8. More friendly ban command

This ban command `/zban` supports offline ban, bans account, ip, uuid at the same time. It supports fuzzy search for names, but it will only ban when the only player is found. If the fuzzy search finds more than one player, in order to prevent accidental ban , this command will ask you to re-enter

### 9. Allow export of player character saves

The `/zout` series of commands allows you to export character data and package the folder according to the current map name. Except for missing character data, which will cause the export to fail, everything else can be exported.

### 10. freeze player

The `/zfre name` command can directly freeze the player. It compares the three data of account, ip, and uuid. If they match, it will be frozen directly. Offline players can be frozen, taking effect when they re-enter the server. Note that this function becomes invalid after the server is restarted and is only used for temporary freezing. For long-term use, please use `/zban`可以冻结离线玩家，当他们再次进入服务器时起效。注意，此功能在服务器重启后失效，仅用于临时冻结，若要长期请使用`/zban`

### 11. Clean up invalid server data

这个功能打算以后从这个插件里移除，因为有点跑题。This feature is planned to be removed from this plugin in the future because it is a bit off topic. Using the `/zclear` command will clear all non-Boss and non-town NPCs in the world in 20 seconds, clean up all items dropped on the ground, clean up all projectiles, and reduce invalid data in the server.

### 12. Statistics of player’s damage output when killing Boss

When the player kills the Boss, this plug-in can count the output of each player and broadcast the output. Players can watch their own combat contributions and even set which mobs to count, such as blue slimes.

## instruction

| Function Categories                 | Permissions    | Commands & Functions                                                                                                                |
| ----------------------------------- | -------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Help series**                     | `zhipm.help`   | `/zhelp`: View help for all instructions under this plugin                                                                              |
| **Save series**                     | `zhipm.save`   |                                                                                                                                                         |
|                                     |                | `/zsave`: Back up your own character save                                                                                               |
|                                     |                | `/zvisa [num]`: View your own backed up inventory, `num` ranges from 1~5 (default 5, can be changed) |
|                                     |                | `/zvisa [name] [num]`: View someone's backed up inventory, `num` ranges from 1~5                                        |
|                                     |                | `/zsaveauto [num]`: Allow users to automatically back up every `num` minutes                                                            |
| **Backup series**                   | `zhipm.back`   | `/zback [name] [num]`: Let the player go back to backup number `num`                                                                    |
| **Copy series**                     | `zhipm.clone`  | `/zclone [name1] [name2]`: Copy the data of `name1` to `name2`                                                                          |
| **Modify series**                   | `zhipm.modify` |                                                                                                                                                         |
|                                     |                | `/zmodify help`: View help for the `zmodify` series of instructions                                                                     |
|                                     |                | `/zmodify [name] life [num]`: Modify health value                                                                                       |
|                                     |                | `/zmodify [name] lifemax [num]`: Modify life limit                                                                                      |
|                                     |                | `/zmodify [name] mana [num]`: Modify mana value                                                                                         |
|                                     |                | `/zmodify [name] manamax [num]`: Modify the maximum mana value                                                                          |
|                                     |                | `/zmodify [name] fish [num]`: Modify the number of angler quest completions                                                             |
|                                     |                | `/zmodify [name] torch [0 or 1]`: Turn on or off the Torch God buff                                                                     |
|                                     |                | ... (Omit other buff instructions)                                                   |
|                                     |                | `/zmodify [name] point [num]`: Modify points                                                                                            |
| **Freeze series**                   | `zhipm.freeze` |                                                                                                                                                         |
|                                     |                | `/zfre [name]`: freeze player                                                                                                           |
|                                     |                | `/zunfre [name]`: Unfreeze player                                                                                                       |
|                                     |                | `/zunfre all`: Unfreeze all players                                                                                                     |
| **Reset series**                    | `zhipm.reset`  |                                                                                                                                                         |
|                                     |                | `/zresetdb [name]`: Reset backup data                                                                                                   |
|                                     |                | `/zresetdb all`: Reset all backup data                                                                                                  |
|                                     |                | `/zresetex [name]`: Reset extra data                                                                                                    |
|                                     |                | `/zresetex all`: Reset all extra data                                                                                                   |
|                                     |                | `/zreset [name]`: Reset character data                                                                                                  |
|                                     |                | `/zreset all`: Reset all character data                                                                                                 |
|                                     |                | `/zresetallplayers`: Reset all data for all players                                                                                     |
| **Check inventory series**          | `zhipm.vi`     |                                                                                                                                                         |
|                                     |                | `/vi [name]`: View player inventory (in order)                                                                       |
|                                     |                | `/vid [name]`: View player inventory (not in order))                                                                 |
| **Check status information series** | `zhipm.vs`     |                                                                                                                                                         |
|                                     |                | `/vs [name]`: View status data                                                                                                          |
|                                     |                | `/vs me`: View your status                                                                                                              |
| **Sort series**                     | `zhipm.sort`   |                                                                                                                                                         |
|                                     |                | `/zsort help`: View sort command help                                                                                                   |
|                                     |                | `/zsort time [num/all]`: Sort online time                                                                                               |
|                                     |                | `/zsort coin [num/all]`: Sort coins by number                                                                                           |
|                                     |                | ... (Omit other sort instructions)                                                   |
|                                     |                | `/zsort clumsy [num/all]`: Sorting noob values                                                                                          |
| **Export data series**              | `zhipm.out`    | `/zout [name/all]`: Export player save                                                                                                  |
| **Ban series**                      | `zhipm.ban`    | `/zban add [name] [Reason]`: optimized ban command                                                                                      |
| **Player game experience settings** | `zhipm.hide`   | `/zhide kill or point`: hide kill or point prompts                                                                                      |
| **Cleaning series**                 | `zhipm.clear`  |                                                                                                                                                         |
|                                     |                | `/zclear useless`: Clean up useless items                                                                                               |
|                                     |                | `/zclear buff [name]`: Clear all player buffs                                                                                           |

## Configuration

> Configuration file location: tshock/Zhipm/ZhiPlayerManager.json

```json5
{
  "是否启用在线时长统计": true,  //"Whether to enable online time statistics": true,  //Enabling this feature will record the player's online time
  "是否启用死亡次数统计": true,  //"Whether to enable death statistics": true,    //Same as above
  "是否启用击杀NPC统计": true,  //"Whether to enable NPC kill statistics": true,  //Same as above
  "是否启用点数统计": false,    //"Whether to enable point statistics": false,   //Points are obtained by killing monsters. It is currently in testing and is turned off by default. You need to turn on "Whether to enable statistics on killing NPCs"
  "默认击杀字体是否对玩家显示": true,  //"Whether the default kill font is displayed to players": true,   //Whether to enable kill + 1 monster killing font, you need to turn on "Whether to enable killing NPC statistics"
  "默认点数字体是否对玩家显示": true,  //"Whether the default point number font is displayed to the player": true,   //The font corresponding to points is currently in testing and is turned off by default. You need to turn on "whether to enable point statistics"
  "是否启用击杀Boss伤害排行榜": true, //"Whether to enable boss damage ranking list": true,   //To count and send the player's damage contribution when killing the Boss, "Whether to enable NPC kill statistics" needs to be turned on
  "是否启用玩家自动备份": false,    //"Whether to enable automatic player backup": false,   //Automatic backup is different from manual backup
  "默认自动备份的时间_单位分钟_若为0代表关闭": 20,  //"Default automatic backup time_unit minute_if 0 means closed": 20,  //Back up online players in the server every 20 minutes
  "每个玩家最多几个备份存档": 5,  //"Maximum number of backup saves per player": 5,   //Maximum number of backup saves per player
  "哪些生物也包含进击杀伤害排行榜":[]  //"Which mobs are also included in the kill damage rankings":[]   //The mobs damage ranking corresponding to the boss damage ranking. You can fill in the mobs ID here. You need to enable "Whether you kill NPC statistics"
  "哪些生物也当成稀有生物进行击杀记录": [],  //"Which creatures are also treated as rare creatures for killing records": []
  "是否允许玩家回溯上次死亡点": true,  //"Whether to allow players to go back to the last point of death": true
  "每次死亡回溯消耗点数": 40,  //"Each death retrospective consumes points.": 40
  "死亡时丢失点数乘数": 1.0,   //"Point multiplier lost on death": 1.0
  "是否允许特殊名字": false    //"Whether special names are allowed": false   //For example, purely numeric names
}
```

## Other

- **Don’t give players backup and rollback permissions at the same time** ，This way they can farm items freely.
- The essence of this plug-in is a comprehensive management collection for players，The reason for its large size is that there are a lot of instructions and it does not take up too much server computing power.
- This plug-in adds two tables to tshock.sqlite，Zhipm_PlayerBackUp and Zhipm_PlayerExtra, the former table is a backup of the tsCharater table，The primary key is AccAndSlot, a string of xxx-x consisting of the player account ID and the backup slot ID，Backup slot ID 1 ~ 5, can be modified in the configuration file
- The latter table records the statistical information of this plug-in: time online time unit seconds，backuptime is the automatic backup time unit in minutes, killNPCnum is the number of NPCs killed.，killBossID The combination of kill BossID and number，For example, 4~10 means killing the Eye of Cthulhu 10 times，killRareNPCID is the combination of the ID and number of rare NPCs killed，point points, hideKillTips whether to hide the white floating word kill+1，hidePointTips Whether to hide the pink floating word +1$
- 点数是测试功能，相当于货币，优点是避免了泰拉自带的网卡狂刷钱的bug和怪物捡钱的bug(使得钱迅速贬值)，点数可以通过杀怪获得，目的是打算以后以此插件为前置插件用统计信息如点数来实现商品购买等功能，目前默认禁用，你可以启用他
- It looks like there are a lot of command permissions，In fact, it is only recommended to give the default player `zhipm.save`, `zhipm.sort`, `zhipm.vi`, `zhipm.vs` these permissions.，Other instructions can be automatically obtained and used by the supervisor，You can use `/zhelp` to check the usage of all commands in the game. You don’t have to remember all the commands.
- **This plug-in places some simple restrictions on player names: names cannot be pure numbers，The name cannot be exactly equal to some instructions of the server. The first character of the name cannot be a special symbol except[**

## 已统计好的数据

- If you want to take advantage of the data organized by this plugin，Please refer to it for use as a front-end plug-in. I suggest you check the source code. Here is a brief introduction这里简单介绍下

```
long Timer;  Timer, records the running time of the server, unit is 1/60 second
List<MessPlayer> frePlayers;  A collection of frozen players
List<ExtraData> edPlayers { get; set; }  All player data integration part
public class ExtraData
{
    /// Account ID
    int Account;
    /// name
    string Name;
    /// Total online time, in seconds
    long time;
    /// Backup interval in minutes
    int backuptime;
    /// Number of mobs killed
    int killNPCnum;
    /// Statistics of ids of bosses killed, id -> number of kills
    Dictionary<int,int> killBossID;
    /// ID statistics of rare mobs killed, id -> number of kills
    Dictionary<int,int> killRareNPCID;
    /// Points (a test feature, equivalent to currency)
    long point;
    /// Whether to hide the words kill + 1
    bool hideKillTips;
    /// Whether to hide words with points + 1 $
    bool hidePointTips;
    ///Number of deaths
    int deathCount;
}
```

## Change log

```
v.1.0.1.1
Fix incorrect GetString
v.1.0.0.9
Modify the default reason for zban and ban user name
v.1.0.0.8
fix: allow non-SSC use
v.1.0.0.6
Improve the uninstall function
v.1.0.0.5
Added a master switch for allowing special names
v.1.0.0.4
Fix that mysql does not specify the key length, modify the Chinese variables to English, and re-add permissions for instructions that lack permissions.
```

## Feedback

- Prioritize issue -> jointly maintained plug-in library: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
