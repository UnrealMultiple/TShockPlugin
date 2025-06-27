# CaiBotLitePlugin 官方机器人适配插件

- 作者: Cai
- 仓库: 此仓库
- 此插件为CaiBotLite适配插件

## 使用指南

https://docs.terraria.ink/zh/caibot/CaiBotLite.html

## 指令

| 语法                           |      权限      |      说明      |
|------------------------------|:------------:|:------------:|
| /caibotlite(cbl) debug       | caibot.admin |    调试模式开关    |
| /caibotlite(cbl) code	       | caibot.admin |    生成验证码     |
| /caibotlite(cbl) info	       | caibot.admin | 显示CaiBot状态信息 |
| /caibotlite(cbl) unbind	     | caibot.admin |   主动解除群绑定    |
| /caibotlite(cbl) whitelist	  | caibot.admin |    开关白名单     |
| /caibotlite(cbl) group <群号>	 | caibot.admin |  设置白名单提示群号   |

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

### v2025.06.04.1 
- 添加命令`/cbl group <群号>`和`/cbl whitelist`方便配置插件
- 默认显示报错调用栈
### v2025.04.30.1 
- 更新依赖SixLabors.ImageSharp
### v2025.04.26.1 
- 修复无法正确处理登录包
### v2025.04.12.1 
- 修复死亡后无法掉落钱币
- 维度插件无法正常获取玩家IP
### v2025.03.29.1 
- 修复白名单登录时空引用报错
### v2025.03.15.1 
- 修改一些提示
### v2025.03.10.1 
- 适配TS 5.2.3
### v2025.01.28.1 
- 建立项目

## 反馈

- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love

