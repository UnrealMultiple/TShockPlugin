# CaiBotPlugin 官方机器人适配插件

- 作者: Cai
- 仓库: 此仓库
- 此插件为CaiBotLite适配插件

## 使用指南

https://docs.terraria.ink/zh/caibot/CaiBotLite.html

## 指令

| 语法                  |      权限      |      说明      |
|---------------------|:------------:|:------------:|
| /caibotlite debug   | caibot.admin |    调试模式开关    |
| /caibotlite code	   | caibot.admin |    生成验证码     |
| /caibotlite info	   | caibot.admin | 显示CaiBot状态信息 |
| /caibotlite unbind	 | caibot.admin |   主动解除群绑定    |

## 配置

> 配置文件位置：tshock/CaiBotLite.json

```json5
{
  "白名单开关": true, //Cai白名单的开关
  "密钥": "1145141919810", //由系统自动配置
  "白名单拦截提示的群号": 991556763 //为0时不显示群号
}
```

## 更新日志

```
v2025.03.15.1 修改一些提示
v2025.03.10.1 适配TS 5.2.3
v2025.01.28.1 建立项目
```

## 反馈

- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love

