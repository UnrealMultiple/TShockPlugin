#Proxyprotocolsocket

- - 作者: [Lavarrow](https://github.com/LaoSparrow) 
- Source: https://github.com/laosparrow/proxyprotocolsocket
- Let TSHOCK support Proxy Protocol, get the user's real IP
- It needs to be used with FRP, Nginx and other proxy software for opening the proxy protocol function
- After installing this plug -in, the game client can only connect TSHOCK through the proxy software, and directly connect to TSHOCK to be rejected

## Update log

```
v2.1
重构 Parsers

v2.0
适配 TShock 5.2
```

## instruction

|grammar|Authority|illustrate|
|:-:|:-:|:-:|
|none|none|none|

## Configuration
> Configure file path: TSHOCK/ProXYProtocolsocket.json
```json
{
   "Settings": {
     "log_level": "Info",
     "timeout": 1000
  }
}
```

|Parameter|illustrate|Scope|default value|
|: ------------|------------------------------------------------------------------------------------------------------------------------------------------------------------|: -----------------------------------------------------------------------------:|: ----:|
|log_level|Terminal log level|None, error, warning, info, debug|Info|
|Timeout|The time of the time of the timeout (the time before receiving the PROXY Protocol data), -1 means that the overtime function is not enabled|             -1 或 正整数              |1000|

## feedback
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love