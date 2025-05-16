# TransfersPatch 翻译工具

- 作者: 少司命
- 出处: 此仓库
- 通过修改程序集实现翻译配置类
- 他能对大部分插件进行翻译

## 指令

```
暂无
```

## 配置
> 配置文件位置：tshock/TransferPatch.json
```json5
{
  "启用": true,
  "目标程序集": "ServerPlugins/TShockAPI.dll",
  "目标类": [ "TShockAPI.Configuration.TShockSettings", "TShockAPI.Configuration.ConfigFile`1" ],
  "翻译列表": {
    "TShockAPI.Configuration.TShockSettings.ServerPassword": "服务器密码",
    "TShockAPI.Configuration.ConfigFile`1.Settings":  "设置" //泛型类(`1`代表泛型参数)
    "Config/Option": "配置" //嵌套类
  }
}
```

## 更新日志

```
暂无
```

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
