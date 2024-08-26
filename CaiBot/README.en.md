# Caibot adaptation plugin

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: CAI
- Warehouse: This warehouse
- CAI Cloud Black Robot adaptation plug -in
- Use \ -Caidebug to turn on the debug mode
- Use \ -Cailocalbot to open the local BOT mode (robot server will point to 127.0.0.1)

## Caibot detailed documentation

- - [Acactt/caibotdocument](https://github.com/ACaiCat/CaiBotDocument) 

## Update log

```
v1.1 修复前置加载失败的问题
v2024.6.7.0 添加自动获取群号，向BOT发送服务器的一些信息(平台、白名单状态、服务器进服密码、插件版本、TShock版本、Terraria版本)，将调试模式(-caidebug)和本地BOT模式(-cailocalbot)分离
v2024.6.19.1 支持共享服务器(Beta测试)
v2024.7.11.1 支持下载地图和下载小地图
v2024.7.24.1 优化代码、发送心跳包、修复进度显示错误
v2024.7.24.2 修复心跳包错误
v2024.7.25.1 更新: CaiBot并行处理数据包，以防止API调用超时
v2024.8.1.1 删除不必要的依赖
v2024.8.1.2 修改Caibot端口
v2024.8.2.1 添加被动添加BOT
v2024.8.25.1 发送base64前用gzip压缩
```

## instruction

```
暂无  
```

## Configuration

> Configuration file location: TSHOCK/CAIBOT.JSON

```json
{
   "密钥": "", //由系统自动配置
   "白名单开关": false, //Cai白名单的开关
   "白名单拦截提示的群号": 0 //白名单拦截提示群号(为0则使用BOT自动获取的群号)
}
```

----------

## How to use (lightweight version):

1. Install the plug -in in serverplugins/

2. Restart server

3. The authorization code will be displayed after starting the server

4. Send the add server <P address> <port> <verification code>

5. Complete configuration

----------

## feedback

- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love
