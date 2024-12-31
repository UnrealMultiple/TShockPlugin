# HelpPlus

- Authors: Cai
- Source: Here
  Fix the issue where the /help command cannot be used in non-English Lang.
  Add a brief prompt for /help.
  Use /help <command> to see more detailed command help.


## Commands

| Command         | Permission |              Details               |
|-----------------|:----------:|:----------------------------------:|
| help <page>     |     no     |     view the list of commands      |
| /help <command> |     no     | view detailed help for the command |

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

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
