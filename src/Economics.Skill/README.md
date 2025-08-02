# Economics.Skill 技能插件

- 作者: 少司命
- 出处: 无
- 一个可以释放技能的插件

> [!NOTE]  
> 需要安装前置插件：EconomicsAPI、Economics.RPG (本仓库) 
> 使用AI样式功能可能会造成占用大量带宽量力而为

## 配置注意事项

- 触发模式中 `击杀` `击打` `主动` 三个模式，其中任意两个都无法组合，击杀 击打 本身就是主动的体现。

## 触发模式
- CD 主动 打击 击杀 死亡 血量 蓝量 冲刺 装备 跳跃 BUFF 技能 环境

## 指令

| 语法                      |          权限           |             说明             |
|-------------------------|:---------------------:|:--------------------------:|
| /skill buy [技能索引]       |  economics.skill.use  |            购买技能            |
| /skill del [技能索引]       |  economics.skill.use  |            解绑技能            |
| /skill ms               |  economics.skill.use  |          查看已绑定技能           |
| /skill delall           |  economics.skill.use  |        解绑手持武器的所有技能         |
| /skill clear            |  economics.skill.use  |           解绑所有技能           |
| /skill reset            | economics.skill.admin |            重置技能            |
| /skill give [玩家] [技能索引] | economics.skill.admin | 给予玩家技能(不安全的，此指令不会检查玩家技能情况) |
| /skill del [玩家] [技能索引]  | economics.skill.admin |          移除目标玩家技能          |
| /skill clearh [玩家]      | economics.skill.admin |         移除玩家所有隐藏技能         |

## 进度限制
Economics文档：[进度限制值](../Economics.Core/README.md)

## 配置
> 配置文件位置：tshock/Economics/Skill.json
```json5
{
  "绑定技能最大数量": 6,
  "单武器绑定最大技能数量": 6,
  "禁止拉怪表": [],  //填写npcid
  "禁止伤怪表": [],  //填写npcid
  "最大显示页": 20,
  "技能列表": [
    {
      "名称": "测试技能",
      "喊话": "",
      "技能唯一": false,
      "全服唯一": false,
      "隐藏": false,
      "技能价格": [  //此技能价格需要根据EconomicsAPI调整
        {
          "数量": 0,
          "货币类型": "经验"
        }
      ],
      "限制等级": [], //填写RPG插件等级，此限制会检测并包含父等级
      "限制进度": [], //填写进度
      "限制技能": [], //技能序号
      "事件循环": {
        "循环间隔": 10,
        "循环次数": 1,
        "弹幕循环": [
          {
            "运行累计触发": 1,
            "运行区间始": 1,
            "运行区间终": 1,
            "弹幕ID": 0,
            "伤害": 0,
            "击退": 0.0,
            "角度": 0,
            "X轴起始位置": 0,
            "Y轴起始位置": 0,
            "X轴速度": 0.0,
            "Y轴速度": 0.0,
            "自动方向": true,
            "持续时间": -1,
            "动态伤害": false,
            "AI": [
              0.0,
              0.0,
              0.0
            ],
            "速度": 0.0,
            "更新事件": [],
            "生成事件": [],
            "广播": ""
          }
        ],
        "范围循环": [
          {
            "运行累计触发": 1,
            "运行区间始": 1,
            "运行区间终": 1,
            "范围": 0,
            "Buff列表": [],
            "清理弹幕": false,
            "命令": [],
            "血量": 0,
            "魔力": 0,
            "NPC增益": [],
            "拉怪": false,
            "拉怪X轴调整": 0,
            "拉怪Y轴调整": 0,
            "伤害敌怪": 0,
            "广播": ""
          }
        ],
        "玩家循环": [
          {
            "运行累计触发": 1,
            "运行区间始": 1,
            "运行区间终": 1,
            "传送": {
              "启用": false,
              "面向修正": false,
              "X轴位置": 0,
              "Y轴位置": 0
            },
            "无敌": {
              "启用": false,
              "时长": 0
            },
            "广播": ""
          }
        ]
      },
      "触发设置": {
        "触发模式": [
          "主动",
          "CD"
        ],
        "冷却": 0,
        "血量": 0,
        "血量比例计算": false,
        "大于血量": false,
        "蓝量": 0,
        "蓝量比例计算": false,
        "大于蓝量": false,
        "物品条件": [
          {
            "背包物品": false,
            "装备": false,
            "饰品": false,
            "手持物品": false,
            "是否消耗": false,
            "物品ID": 0,
            "数量": 0,
            "前缀": 0
          }
        ],
        "Buff条件": [],
        "环境条件": [],
        "技能条件": []
      },
      "执行脚本": "环绕锁敌.js" //执行脚本名称
    }
  ]
}
```

## 配置说明
- 在上方示例配置中，我并未解释事件循环如何配置，我个人认为事件循环的设计过于复杂，因此我并不推荐使用配置来实现弹幕效果
- 在最新版的Skill插件配置中更加注重对单个弹幕的控制，这可能有助于实现更加复杂的弹幕效果，但他让配置文件变的宂余。因此我添加了新选择`JavaScript`
- 这是我期待的，触发效果交给配置文件来处理，当达成触发条件后调用js脚本。

## 脚本(Script)
脚本是通过Jint库进行调用的，其性能并不高，我考虑过使用ClearScriptV8来支持脚本运行，但对于一个插件来说它太大了，因此最终选择了Jint，如果将来遇到性能瓶颈，我会考虑升级至ClearScriptV8.

先来看一个示例
```javascript
var Xna = importNamespace("Microsoft.Xna.Framework");
var Terraria = importNamespace("Terraria");
/*
* skill 是技能配置对象
* ply 是玩家对象
* pos 是技能释放位置
* vel 是技能释放方向
* identify 弹幕释放id如果不是弹幕触发的则为-1
*/
function main(skill, ply, pos, vel, identify) {
    let projs = [];
    var distance = 16 * 25;    // 固定半径
    var startAngle = 0;         // 初始角度（度）
    var angleIncrement = 60;    // 角度间隔（度）
    var count = 360 / angleIncrement;             // 弹幕数量

    // 生成初始位置并记录每个弹幕的初始角度（弧度）
    var posList = ply.TPlayer.Center.GetPointsOnCircle(distance, startAngle, angleIncrement, count);
    var n = 0;
    posList.ForEach(pos => {
        var projIdx = SpawnProjtile(ply, pos, new Xna.Vector2(0, 0), 684, 100, 10, ply.Index, 0, 0, 0);
        SendProjectilePacket(projIdx);
        projs.push({
            id: projIdx,
            angle: (startAngle + n * angleIncrement) * Math.PI / 180 // 初始角度转为弧度
        });
        n += 1;
    });
    
    var loopCount = 60 * 100;
    var j = 0;
    for (let i = 0; i < loopCount; i++) {
        Schedule((args) => {
            j++; //闭包原因无法正确捕获变量i用j代替
            projs.forEach(projData => {
                var proj = Terraria.Main.projectile[projData.id];
                //if (!proj.active) return;

                // 计算新角度：初始角度 + 旋转速度 * 当前帧
                var newAngle = projData.angle + j * 0.05; // 每帧旋转0.05弧度
                var center = ply.TPlayer.Center;

                // 用固定半径计算新位置
                proj.Center = new Xna.Vector2(
                    center.X + Math.cos(newAngle) * distance,
                    center.Y + Math.sin(newAngle) * distance
                );
                SendProjectilePacket(projData.id);
                LockNpc(ply, proj, projData.id);
            });
        }, i); // 按帧延迟执行
    }
}

function LockNpc(ply, proj, id){
    var npc = proj.Center.FindRangeNPC(50 * 16);
    if (npc && Terraria.Main.rand.Next(40) == 0) {
       // 找到最近的怪物
        let targetVel = Terraria.Main.projectile[id].Center.DirectionTo(npc.Center).SafeNormalize(Xna.Vector2.UnitY);
        targetVel = targetVel.ToLenOf(10);
        var projIdx = SpawnProjtile(ply, proj.Center, targetVel, 684, 100, 10, ply.Index);
        SendProjectilePacket(projIdx);
    }
}
```
- 上方的js脚本，会创建一些弹幕环绕在玩家身边持续100秒时间，在此期间环绕中的弹幕会锁定并攻击附近50格范围内的怪物。
### 脚本说明
所有的脚本必须已以下格式编写
```javascript
function main(skill, ply, pos, vel){

}
```
这就跟c系语言的入口函数一样，main函数是脚本的入口，其参数亦不可变。

#### 在脚本中内置了一些函数
```javascript
//void log(object msg)

function main(skill, ply, pos, vel){
    log("Hellow World"); //在ts控制台你会看到输出
}
```
```javascript
//int SpawnProjtile(TSPlayer ply, Vector2 pos, Vector2 vel, int type, int Damage, int KnockBack, int Owner, float ai0 = 0, float ai1 = 0, float ai2 = 0, int timeLeft = -1, string uuid = "")

function main(skill, ply, pos, vel){
    let index = SpawnProjtile(ply, pos, vel, 684, 100, 10, ply.Index, 0, 0, 0)
        SendProjectilePacket(index);
}
```
```javascript
//void SendPacket(int packetid, int num, int num2, int num3, int num4, int num5, int num6, int num7)

function main(skill, ply, pos, vel){
    SendPacket(66, ply.Index, life) //回复玩家100点生命
}
```
```javascript
//void Schedule(Action<object> action, int interval, object obj)

function main(skill, ply, pos, vel){
    Schedule((args) =>{
        SendPacket(66, ply.Index, life);
    }, 60); //延迟60帧(1秒) 给玩家回复深生命值。
}
```
#### 支持的扩展函数
- EconomicsAPI.Extensions.GameProgress
- Terraria.Utils
- EconomicsAPI.Extensions.PlayerExt
- EconomicsAPI.Extensions.NpcExt
- EconomicsAPI.Extensions.TSPlayerExt
- Enumerable
- EconomicsAPI.Extensions.Vector2Ext
示例脚本中相当一部分代码使用了扩展函数，你可以在这些命名空间中找到具体函数

#### 支持的Assembly
- EconomicsAPI
- TShockAPI
- OTAPI
- System.Thread.Task
你可以通过`importNamespace`函数访问这些程序集中的公开类

## 更新日志
### v2.0.1.0
- 添加 GetString

### v2.0.0.9
- 技能条件，buff条件，环境条件
- 购买技能条件

### v2.0.0.0
- 适配多货币

### v1.2.1.6
- 添加隐藏技能，隐藏技能无法被主动购买。
- 添加新指令:
  - /skill give 给玩家添加技能，此指令不是一个安全的指令，它不会检查玩家技能状态。
  - /skill del 这个指令可以删除目标玩家技能
  - /skill clearh 移除目标玩家身上的隐藏技能

### v1.2.1.5
- 适配新 EconomicsAPI

### v1.1.0.1
- 添加 无敌帧，锁定怪物，AI样式，传送玩家，移除画圆配置，改用循环实现
- 修复: 弹幕AI无法生效，持续时间无法生效

### V1.0.0.1
- 修复:物品消耗

## 反馈

- 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 国内社区 trhub.cn 或 TShock 官方群等
