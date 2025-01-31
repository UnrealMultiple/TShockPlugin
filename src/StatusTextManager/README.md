# StatusTextManager 模板文本管理器
- 作者: LaoSparrow
- 出处: https://github.com/CNS-Team/StatusTxtMgr
- 管理 PC 端右上角的模板文本


## 指令

| 语法          | 权限 |    说明    |
|-------------|:--:|:--------:|
| /statustext | 无  | 切换模板文本显示 |
| /st         | 无  | 切换模板文本显示 |

## DynamicText 插值表

| 插值                        | 插值内容              |
|---------------------------|-------------------|
| {PlayerName}              | 玩家名称              |
| {PlayerGroupName}         | 玩家所在组名称           |
| {PlayerLife}              | 玩家生命值             |
| {PlayerMana}              | 玩家魔力值             |
| {PlayerLifeMax}           | 玩家最大生命值           |
| {PlayerManaMax}           | 玩家最大魔力值           |
| {PlayerLuck}              | 玩家幸运值             |
| {PlayerCoordinateX}       | 玩家X坐标             |
| {PlayerCoordinateY}       | 玩家Y坐标             |
| {PlayerCurrentRegion}     | 玩家所在的 TShock 区域   |
| {IsPlayerAlive}           | 玩家是否存活            |
| {RespawnTimer}            | 玩家重生倒计时 (未知原因不生效) |
| {OnlinePlayersCount}      | 在线玩家数量            |
| {OnlinePlayersList}       | 在线玩家列表            |
| {AnglerQuestFishName}     | 渔夫任务鱼名称           |
| {AnglerQuestFishID}       | 渔夫任务鱼ID           |
| {AnglerQuestFishingBiome} | 渔夫任务鱼钓鱼点          |
| {AnglerQuestCompleted}    | 渔夫任务是否已完成         |
| {CurrentTime}             | 游戏内时间             |
| {RealWorldTime}           | 现实世界时间            |
| {WorldName}               | 世界名称              |
| {CurrentBiomes}           | 玩家当前所处群系          |

## 配置
> 配置文件位置：tshock/StatusTextManager.json
```json5
{
  "Settings": {
    "LogLevel": "INFO", //日志等级
    "StatusTextSettings": [ //模板文本配置
      {
        "TypeName": "StaticText", //静态文本类型
        "Text": "Helloooooooooooooooooooooooooo\n" //静态文本内容
      },
      {
        "TypeName": "HandlerInfoOverride", //插件模板文本配置覆盖
        "PluginName": "STMTest2", //插件的 AssemblyName, 一般是插件 dll 去掉扩展名
        "Enabled": true, //是否启用插件模板文本
        "OverrideInterval": true, //是否覆盖插件模板文本更新间隔
        "UpdateInterval": 1200 //更新间隔, 以帧为单位, 60=1s, 比如这里 1200=20s 
      },
      {
        "TypeName": "DynamicText", //动态文本类型
        "Text": "\nWorld Name: {WorldName}, {Player Name: {PlayerName}}, Field: {{PlayerName}}\n", //动态文本内容， 被花括号{}包裹的插值会被动态替换成对应的插值内容, 用双花括号跳过插值
        // 如果想要做到 Player Name: {Sparrow} 还是用两个 StaticText 包裹吧。。
        "UpdateInterval": 60 //更新间隔, 大同小异
      },
      {
        "TypeName": "HandlerInfoOverride", //大同小异
        "PluginName": "STMTest1",
        "Enabled": true,
        "OverrideInterval": true,
        "UpdateInterval": 600
      }
    ]
  }
}
```
最终显示效果
```
Helloooooooooooooooooooooooooo
Sparrow Hello from STMTest2 9
World Name: 1449World, {Player Name: Sparrow}, Field: {PlayerName}
Sparrow Hello from STMTest1 16
```
示例配置文件
```json5
{
  "Settings": {
    "LogLevel": "INFO",
    "StatusTextSettings": [
      {
        "TypeName": "DynamicText",
        "Text": "\n\n\n\n\n\n\n\n\n\n--[提[i:29]瓦[i:29]特]--\n[i:1503]玩家名称: {PlayerName}\n[i:346]当前组别: {PlayerGroupName}\n[i:893]当前世界: {WorldName}\n[i:855]幸运值: {PlayerLuck}\n[i:889]游戏时间: {CurrentTime}\n[i:{AnglerQuestFishID}]渔夫任务鱼: {AnglerQuestFishName}\n[i:3036]任务鱼钓鱼点: {AnglerQuestFishingBiome}[i:1307]\n[i:267]在线玩家: {OnlinePlayersList}\n[i:3122]当前群系: {CurrentBiomes}",
        "UpdateInterval": 60
      }
    ]
  }
}

```

## 适配示例代码

```csharp
using Terraria;
using TerrariaApi.Server;

namespace STMTest1;

[ApiVersion(2, 1)]
// ReSharper disable once UnusedType.Global
public class Plugin : TerrariaPlugin
{
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public Plugin(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        StatusTextManager.Hooks.OnStatusTextUpdate.Register(this.OnStatusTextUpdate, 60);
    }

    private int _counter;
    private void OnStatusTextUpdate(StatusTextManager.StatusTextUpdateEventArgs args)
    {
        args.StatusTextBuilder.AppendFormat($"{args.TSPlayer.Name} Hello from STMTest1 {this._counter++}");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            StatusTextManager.Hooks.OnStatusTextUpdate.Deregister(this.OnStatusTextUpdate);
        }
        base.Dispose(disposing);
    }
}
```

## 更新日志

```
v1.1.3
重构渔夫任务点提取逻辑

v1.1.0
添加 DynamicText 类
可插值

v1.0.0
初次添加插件
```

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
