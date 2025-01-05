# Chameleon

- Author: mistzzt, repaired by 肝帝熙恩 for issue 1449
- Source: [github](https://github.com/mistzzt/Chameleon)
- Players entering the server for the first time will receive a prompt and be asked to log out and log back in.
- The next time they enter the server, they will be prompted to enter a password, which will be saved as their user password (equivalent to registration).
- Afterwards, if players switch devices (or UUID), or if there are name conflicts, or if their IP changes when entering the game, they will be required to re-enter their user password.

## Commands

```
None
```

## Config

> Configuration file location：tshock/Chameleon.en-US.json

```json
{
  "VerifyloginIP": false,
  "AwaitBufferSize": 10,
  "EnableForcedHint": true,
  "Greeting": "   Welcome to niaoluo Server!",
  "VerificationFailedMessage": "         Incorrect username or password.\r\n\r\n         If it's your first time joining, please choose a different character name;\r\n         If you have forgotten your password, please contact the administrator.",
  "Hints": [
    " ↓↓ Please read the following hints to enter the server ↓↓",
    " \r\n         Finish reading before clicking →",
    " 1. Enter your own password in the \"Server Password\" field;\n   this is equivalent to setting a password for your account.\n   Use this password when joining the server in the future.",
    " 2. After reading, please rejoin the server."
  ],
  "SyncServerReg": [
    {
      "Name": "Test Server",
      "IpAddress": "http://127.0.0.1:7878/",
      "Token": "TOKENTEST"
    }
  ]
}
```

## 更新日志

```
v1.0.8
修正英文翻译
v1.0.5
添加ip验证，如果玩家更换ip会要求重新输入密码
V1.0.4
使用lazyapi
v1.0.3
i18n和README_EN.md
v1.0.2
i18n预定
v1.0.1
修复了reload后同步注册服务器反复写入的问题
```

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
