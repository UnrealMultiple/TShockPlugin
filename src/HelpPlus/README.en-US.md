# HelpPlus

- Authors: Cai
- 出处: 此仓库
- 修复/help指令无法使用的问题
- 为/help添加简短提示
- view detailed help for the command

## Commands

```
/help <command>
```

## Config

> Configuration file location：tshock/HelpPlus.json

```json5
{
  "简短提示开关": true, //Enable brief prompt
  "简短提示对应": { //Brief prompt setting
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

```
- v2024.9.1.1 更新翻译
- v2024.7.28.1 修复/death、/roll等原版命令
- v1.0.0 修复Help需要权限的奇怪问题
```

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
