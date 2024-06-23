# CaiBot适配插件

- 作者: Cai  
- 仓库: 此仓库
- Cai云黑机器人的适配插件
- 使用 \-caidebug开启调试模式
- 使用 \-cailocalbot开启本地BOT模式 (机器人服务器会指向127.0.0.1)
## CaiBOT详细文档

- [ACaiCat/CaiBotDocument](https://github.com/ACaiCat/CaiBotDocument)
## 更新日志

```
v1.1 修复前置加载失败的问题
v2024.6.7.0 添加自动获取群号，向BOT发送服务器的一些信息(平台、白名单状态、服务器进服密码、插件版本、TShock版本、Terraria版本)，将调试模式(-caidebug)和本地BOT模式(-cailocalbot)分离
```

## 指令

```
暂无  
```

## 配置
> 配置文件位置：tshock/CaiBot.json
```json
{
  "密钥": "", //由系统自动配置
  "白名单开关": false, //Cai白名单的开关
  "白名单拦截提示的群号": 0 //白名单拦截提示群号(为0则使用BOT自动获取的群号)
}
```

----------
## 使用方法(轻量版):

1.将插件安装在ServerPlugins/   

2.重启服务器  

3.启动服务器后会显示授权码  

4.群内发送 添加服务器 <IP地址> <端口> <验证码>  

5.完成配置

----------

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/Controllerdestiny/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love

