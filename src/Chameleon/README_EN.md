# Chameleon 

- Author: mistzzt, repaired by 肝帝熙恩 for issue 1449
- Source: [github](https://github.com/mistzzt/Chameleon)
- Players entering the server for the first time will receive a prompt and be asked to log out and log back in.
- The next time they enter the server, they will be prompted to enter a password, which will be saved as their user password (equivalent to registration).
- If the player changes devices (or UUID) or if someone with the same name enters the game, they will be required to re-enter their user password.


## Commands

```
None
```

## Config
> Configuration file location：tshock/Chameleon.json
```json
{
  "等待列表长度": 10,  // Waiting list length
  "启用强制提示显示": true,  // Enable forced prompt display
  "强制提示欢迎语": "   欢迎来到茑萝服务器！",  // Forced prompt welcome message
  "验证失败提示语": "         账户密码错误。\r\n\r\n         若你第一次进服，请换一个人物名；\r\n         若忘记密码，请联系管理。",  // Verification failure prompt
  "强制提示文本": [  // Forced prompt text
    " ↓↓ 请看下面的提示以进服 ↓↓",  // Please read the following prompt to enter the server
    " \r\n         看完下面的再点哦→",  // Read the following and then click
    " 1. 在\"服务器密码\"中输入自己的密码, 以后加服时输入这个密码即可",  // 1. Enter your password in the "Server Password" field, and use this password when joining the server in the future
    " 2. 阅读完毕后请重新加入"  // 2. After reading, please rejoin the server
  ],
  "同步注册服务器": [  // Synchronized registration servers
    {
      "名称": "测试服务器",  // Server Name
      "地址": "http://127.0.0.1:7878/",  // Address
      "Token": "TOKENTEST"  // Token
    }
  ]
}
```

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
