#Helpplus better help

- Author: CAI
- Source: This warehouse
- Fix the problem that the/Help instruction cannot be used
- Add a short reminder to/help
- For/Help <command> you can see more detailed command help

## Update log

```
- v1.0.0 修复Help需要权限的奇怪问题
- v2024.7.28.1 修复/death、/roll等原版命令
```

## instruction

```
/help <页码> 查看命令列表
/help <命令> 查看命令的详细帮助
```

## Configuration

> Configuration file location: TSHOCK/Helpplus.json

```json
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

## feedback

- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love