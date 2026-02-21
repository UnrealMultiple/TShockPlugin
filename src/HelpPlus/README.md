# HelpPlus 更好的帮助

- 作者: Cai,羽学
- 出处: 此仓库
- 修复/help指令无法使用的问题
- 为/help添加简短提示
- 为/help <命令> 可以看见更详细的命令帮助


## 指令

```
/help <页码> 查看命令列表
/help <命令> 查看命令的详细帮助
```

## 配置

> 配置文件位置：tshock/HelpPlus.json

```json5
{
  "简短提示开关": true,
  "简短提示对应": {
    "user": "用户管理",
    "login": "登录",
    "logout": "登出",
    "password": "修改密码",
    "register": "注册",
    "accountinfo": "账号信息",
    "ban": "封禁",
    "broadcast": "广播",
    ......
    "deal": "交易",
    "igen": "快速构建",
    "relive": "复活NPC",
    "bossinfo": "进度查询"
  }
}
```

## 更新日志
### v2025.7.19
- 优化命令列表格式，增加颜色使其更加分明，新增更多配置项
### v2024.9.1.1
- 更新翻译
### v2024.7.28.1
- 修复/death、/roll等原版命令
### v1.0.0
- 修复Help需要权限的奇怪问题

## 反馈

- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
