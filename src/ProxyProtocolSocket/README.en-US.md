# ProxyProtocolSocket

- Author: [LaoSparrow](https://github.com/LaoSparrow)
- Source: https://github.com/LaoSparrow/ProxyProtocolSocket
- Let TShock support `proxy protocol`, obtain the user's real IP
- It needs to be used with anti-generation software such as `frp` and `nginx` that have enabled the `proxy protocol` function
- After installing this plugin, the game client can only connect to TShock through anti-generation. If you connect to TShock directly, you will be denied access.

## Instruction

```
None
```

## Configuration
> Configuration file path: tshock/ProxyProtocolSocket.json
```json5
{
  "Settings": {
    "log_level": "Info",
    "timeout": 1000
  }
}
```

| Parameter        | Description                                              |                Value Range                | Default Value  |
|:----------|:------------------------------------------------|:---------------------------------:|:----:|
| log_level | Terminal log level                                          | None, Error, Warning, Info, Debug | Info |
| timeout   | Number of timeout milliseconds (the time to wait before receiving proxy protocol data), -1 means that the timeout function is not enabled |             -1 or positive integer              | 1000 |

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
