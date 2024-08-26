# Chameleon logged in before taking the clothes.

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: mistzzt, Gan Di Xi 'en Repair 1449
- - 出处: [github](https://github.com/mistzzt/Chameleon) 
- Players who take the service for the first time will be prompted and asked to log out and log in again.
- They will be asked to enter a password after the next service, and the password entered this time will be saved, which is equivalent to the player's own user password (equivalent to registration).
- After that, if the player changes the device (or UUID), or someone with the same name enters the game, they will be asked to enter the user password again.
## Update log

```
v1.0.1
修复了reload后同步注册服务器反复写入的问题
```

## instruction

```
暂无
```

## deploy
> Configuration file location: tshock/Chameleon.json
```json
{
   "等待列表长度": 10,
   "启用强制提示显示": true,
   "强制提示欢迎语": "   欢迎来到茑萝服务器！",
   "验证失败提示语": "         账户密码错误。\r\n\r\n         若你第一次进服，请换一个人物名；\r\n         若忘记密码，请联系管理。",
   "强制提示文本": [
     " ↓↓ 请看下面的提示以进服 ↓↓",
     " \r\n         看完下面的再点哦→",
     " 1. 在\" 服务器密码\"中输入自己的密码, 以后加服时输入这个密码即可",
     " 1. 阅读完毕后请重新加入" 
  ],
   "同步注册服务器": [
    {
       "名称": "测试服务器",
       "地址": "http://127.0.0.1:7878/",
       "Token": "TOKENTEST" 
    }
  ]
}
```
## feedback
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.