# ModifyWeapons 修改武器插件

- 作者: 羽学
- 出处: Tshock官方QQ群816771079
- 这是一个Tshock服务器插件，主要用于：修改储存玩家武器数据并自动重读,可使用/mw指令给予玩家指定属性的物品



## 指令

| 语法             |     别名     |        权限         |        说明        |
|----------------|:----------:|:-----------------:|:----------------:|
| /mw            |     无      |      mw.use       |        指令        |
| /mw hand       |   /mw h    |      mw.use       |    获取手持物品信息开关    |
| /mw join       |   /mw j    |      mw.use       |     切换进服重读开关     |
| /mw list       |   /mw l    |      mw.use       |     列出所有修改物品     |
| /mw read       |     无      |      mw.use       |    手动重读所有修改物品    |
| /mw auto       |   /mw at   |      mw.amin      |     自动重读功能开关     |
| /mw clear      |   /mw cr   |      mw.amin      |     自动清理功能开关     |
| /mw open 玩家名   |   /mw op   |     mw.admin      |    切换别人进服重读状态    |
| /mw add 玩家名 次数 |     无      |     mw.admin      |      添加重读次数      |
| /mw del 玩家名    |     无      |     mw.admin      |     删除指定玩家数据     |
| /mw set        |   /mw s    |     mw.admin      |    修改自己手持物品属性    |
| /mw up         | /mw update |     mw.admin      |    修改自己手持物品属性    |
| /mw give       |   /mw g    |     mw.admin      |  给指定玩家修改物品并建数据   |
| /mw all        |     无      |     mw.admin      |  给所有玩家修改物品并建数据   |
| /mw pw         |   /mw p    |     mw.admin      |     公用武器相关修改     |
| /mw reads      |  /mw rds   |     mw.admin      |  帮所有人重读或开启进服重读   |
| /mw reset      |   /mw rs   |     mw.admin      |     重置所有玩家数据     |
| /reload        |     无      | tshock.cfg.reload |      重载配置文件      |
| 无              |     无      |       mw.cd       | 忽略冷却时间与次数的重读武器权限 |

## 配置
> 配置文件位置：tshock/修改武器.json
```json5
{
  "插件开关": true,
  "初始重读次数": 2,
  "只给指定名字物品": true,
  "给完物品的延迟指令": true,
  "延迟指令毫秒": 500.0,
  "延迟指令表": [
    "/mw read"
  ],
  "自动重读": 1,
  "触发重读指令检测表": [
    "deal",
    "shop",
    "fishshop",
    "fs"
  ],
  "清理修改武器(丢出或放箱子会消失)": true,
  "免清表": [
    1
  ],
  "进服只给管理建数据": false,
  "增加重读次数的冷却秒数": 1800.0,
  "启用公用武器": true,
  "同步数据秒数": 15,
  "公用武器播报标题": "羽学开荒服 ",
  "公用武器表": [
    {
      "名称": "火枪",
      "ID": 96,
      "数量": 1,
      "前缀": 82,
      "伤害": 30,
      "大小": 1.0,
      "击退": 5.5,
      "用速": 10,
      "攻速": 15,
      "弹幕": 10,
      "弹速": 9.0,
      "弹药": 0,
      "发射器": 97,
      "颜色": {
        "packedValue": 0,
        "R": 0,
        "G": 0,
        "B": 0,
        "A": 0,
        "PackedValue": 0
      }
    },
    {
      "名称": "夺命枪",
      "ID": 800,
      "数量": 1,
      "前缀": 82,
      "伤害": 35,
      "大小": 1.0,
      "击退": 2.5,
      "用速": 10,
      "攻速": 15,
      "弹幕": 5,
      "弹速": 6.0,
      "弹药": 0,
      "发射器": 97,
      "颜色": {
        "packedValue": 0,
        "R": 0,
        "G": 0,
        "B": 0,
        "A": 0,
        "PackedValue": 0
      }
    }
  ]
}
```

## 更新日志

### v1.2.7.3
- 修复reload不重载配置文件

### v1.2.7.2
- Database改utf-8

### v1.2.7.1
- 修复空引用

### v1.2.7
- 修复了/reload后导致玩家所有修改武器被公用武器覆盖的BUG
- 修复了"公用武器已更新"提示语,仅在数据未同步下会播报
- 加入了给完物品后的延时执行指令(支持up all give 子命令):
- 可避免编写RPG时使用:/mw g {0} 物品数据不能及时同步问题
- 不开启【进服只给管理建数据】情况下： up all give 只需输1遍即可同步数据

### v1.2.6
- 以下功能由556牌服主定制
- 为了方便编写RPG配置，给以下子命令加入玩家自己的占位符：{0}
- up del give all
- 给give与all子命令加入了“只给指定名字物品”配置项
- 开启时只会给指定的物品，反之给玩家数据库内存在的所有修改物品
- 加入了公用武器，根据配置文件编写的物品数据自动给新玩家写入
- 当玩家开荒时捡到这些物品可以手动重读获取新数据，
- 公用武器的数据优先级＞使用指令给的物品数据优先级，
- 如果玩家没有数据时，默认会从配置文件加载一份到数据库内。
- 根据【同步数据秒数】配置项自动更新到玩家数据库内。
- 修改/reload重载逻辑（仅在开启公用武器时有效）：
- 播报玩家数据与公用武器中配置数据的差异性(玩家没拿过那个武器每次重载都会播报那个数值)
- 同时如果配置中没有该武器，重载后会从所有玩家数据移除该武器
- "公用武器播报标题"配置项有任意内容时，会播报多以下信息：
- xxxxx 公用武器已更新
- 加入了新指令/mw pw（空值输入自带格式教学）：
  - 格式1:/mw pw 物品名 da 100 ——修改或添加指定公用武器
  - 格式2:/mw pw on与off 开启关闭公用武器
  - 格式3:/mw pw del 物品名 删除公用武器配置中与所有玩家数据内的指定武器

### v1.2.5
- 加入了自动清理功能与其对应开关指令：`/mw clear`
- 该指令控制配置项 `清理修改武器`
- 并配备1个 `免清表` 配置项过滤使用。
- 拥有 `mw.admin` 管理权限的免疫清理
- 当玩家主动丢出物品或物品放入箱子时会清理修改武器（重读时掉落的不会清）
- 加入了自动重读识别经济指令逻辑(对管理也有效):
- 当玩家发送字符以 `/` 和 `.` 开头时,且后续含有 `触发重读指令检测表` 内的关键词
- 检查玩家背包是否有修改武器,有则触发重读,避免玩家恶意购买物品刷新数值
- 修复一些bug：
- 修复/mw open指令显示不正确
- 修复/mw all不会查找弹药栏有相同物品,则多给一份bug

### v1.2.4
- 优化播报与发送语,补充了修改参数的指令教学
- 移除自动更新的伤害检测逻辑（存在BUG）
- 修复了/mw all指令会重复给身上已有物品的BUG
- 使用/mw read重读时会显示物品身上重读的物品名字与数量

### v1.2.3
- 加入了自动更新判断:玩家是否正在使用物品的前提条件
- 加入了自动更新判断:物品是否为修改的弹药属性
- 加入了物品重读判断:会查找玩家背包是否有对应修改物品才会更新
- 加入了物品颜色属性:格式为16进制不含#号如：/mw s hc CDEEEB
- 加入了离线修改逻辑:/mw all 与 g 与 up
- 无论玩家在线或离线:没数据则自动建,有数据则更新,在线就重读并直接给物品(除/mw up)（/mw up需该玩家已经拥有修改物品前提下才能修改,已支持多参数组合修改）
- 优化/mw reads 命令:reads 1为帮所有在线玩家重读,reads 2为修改所有人进服重读
- 注意：词缀只有在玩家手上没拿着修改物品才会更新，
- 如果玩家在线且手上拿着修改物品，只会写入手上的词缀

### v1.2.2
- 整理优化了代码,补充信息反馈
- 支持修改前缀、物品数量
- 加入了自动重读功能（测试版）：/mw auto
- 开启时会关闭玩家重读次数机制(占用了玩家自己的重读冷却时间)
- 只在手持修改物品时伤害超过修改值+误判值，或者手上物品词缀不对时触发
- 将数据结构从Config搬移到tshock.sqlite存储
- mw.admin有权享受无视重读次数
- 修复了mw.admin权限无法使用各别管理命令BUG：
- 写的时候用的:cmd.admin,结果忘记改了

### v1.2.1
- 加入了/mw all指令给所有在线玩家发指定物品并建立数据
- 玩家收到管理发送物品时会提示准确的修改数值与手动重读提醒
- 修正了/mw read 播报逻辑
- 声明up子命令详情：
 - /mw s 或 g 或 all都会先还原其他数值再改指定数值
 - 而/mw up 玩家名字 物品名 ua 20，这能保留之前的数值直接改指定数值
- 物品属性参数详情（也可以用中文名）:
  - 伤害:d da
  - 大小:c sc
  - 击退:k kb
  - 用速:t ut
  - 攻速:a ua
  - 弹幕:h sh
  - 弹速:s ss
  - 作弹药:m am
  - 用弹药:aa uaa

### v1.2.0
- 重构代码，支持自定义多个武器物品
- 移除了数据库逻辑，改用配置文件来存数据
- 在开启“进服只给管理建数据”配置项时：
- 玩家数据只能用/mw g指令给了一个物品后才会为那个玩家创建数据
- 加入了更多指令：
 - 当空值输入/mw s 或 g 或 up时会提示相对应的教学 （输入成功后会无视玩家自身重读次数，立即更新玩家游戏内物品状态）
 - 使用/mw list支持翻页查找自己所有修改武器数值（一页1个）
 - 使用/mw up指令时只会更新指定玩家物品的唯一参数，前提是必须有修改过的物品

### v1.1.0
- 加入了修改配置内的数据使用/reload也能同步到数据库中
- 如果误删了配置里的数据表,可以重启服务器它会从数据库写回配置文件里

- 加入了组合属性修改逻辑:
  - /mw set d 100 ss 20
  - /mw g 玩家名 物品名 d 100 ss 20

- 加入了重读冷却机制与重读次数(免疫权限为:mw.cd)
- 第一次进服默认都有2次重读次数，而进服重读默认为关闭
- 当冷却达到Config预设的秒数时，自动加1次重读次数
- 根据重读次数可直接使用/mw read 手动重读武器数值
- 如果没有达到冷却或没有重读次数可截图自己的/mw菜单页面给管理
- 让管理在后台使用指令也可以帮玩家手动重读:
  - /mw g 玩家名字 武器名 数值… （这个指令也会主动开启玩家进服重读功能）

- 修复了/mw del 不会删配置文件的BUG
- 修复了/mw open 不会同步配置文件的BUG
- 修复了重铸后再重读武器会吞武器前缀的BUG,
- /mw g 会判断玩家是否手持自定义武器来获取准确前缀 （使用前建议让玩家先选中自定义武器，再让管理员用/mw g）


### v1.0.0
- 羽学版自定义武器含数据库
- 参考/mw指令菜单最下面一行的数值状态来修改
- 修改自己的手持物品：/mw s 200 1 4 20 12 938 10
- 修改并给予指定玩家的物品：/mw g 羽学 铜短剑 200 1 4 20 12 938 10
- 关于弹幕类武器： 有时候降低速度,反而弹幕频率会更密集

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love