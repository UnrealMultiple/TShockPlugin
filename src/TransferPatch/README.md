# TransfersPatch 翻译工具

- 作者: 少司命
- 出处: 此仓库
- 通过修改程序集实现翻译配置类
- 他能对大部分插件进行翻译
- 例如我们可以对ts进行配置上的汉化
- 此插件提供默认配置翻译TShockAPI

## 注意事项
- 插件修改后的结果是永久性的，使用之前最好备份插件。

## 指令

```
| 语法                             |         权限         | 说明                 |
|--------------------------------|:------------------:|:-------------------|
| dump-config <插件名> <类名>       | transferpatch.dump | dump配置类到文件   |
```

## 配置
> 配置文件位置：tshock/TransferPatch.json
```json5
{
  "启用": true,
  "执行列表": [
    {
	  "目标程序集": "ServerPlugins/TShockAPI.dll",
      "目标类": [ "TShockAPI.Configuration.TShockSettings", "TShockAPI.Configuration.ConfigFile`1" ],
      "翻译列表": {
        "TShockAPI.Configuration.TShockSettings.ServerPassword": "服务器密码",
        "TShockAPI.Configuration.ConfigFile`1.Settings":  "设置" //泛型类(`1`代表泛型参数数量)
        "Config/Option": "配置" //嵌套类 (指Option声明在Config类中)
      }
    }
  ]
}
```

## 更新日志

### v1.0.0
- 添加插件

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
