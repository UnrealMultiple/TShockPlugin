# StatusTextManager 模板文本管理器
- 作者: LaoSparrow
- 出处: https://github.com/CNS-Team/StatusTxtMgr
- 管理 PC 端右上角的模板文本

## 更新日志

```
v1.0.0
初次添加插件
```

## 指令

| 语法          | 权限 |    说明    |
|-------------|:--:|:--------:|
| /statustext | 无  | 切换模板文本显示 |
| /st         | 无  | 切换模板文本显示 |

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
        "TypeName": "StaticText", //大同小异
        "Text": "\nAnother Static Text\n"
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
Another Static Text
Sparrow Hello from STMTest1 16
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
    public override string Name => "STMTest1";

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

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
