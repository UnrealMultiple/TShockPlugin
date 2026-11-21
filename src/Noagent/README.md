# Noagent 反代理插件

- 作者: Jonesn，肝帝熙恩
- 出处: TShock中文官方群
- 默认调用`https://blackbox.ipinfo.app/lookup/`进行检测，实测效果还行；也可在配置中改用 [VPNDetection](https://vpndetection.io)（拦截 VPN 和 Tor，无需密钥：[条款](https://vpndetection.io/terms#no-key)、[隐私](https://vpndetection.io/privacy)）

## 指令

```
暂无
```

## 配置
> 配置文件位置：tshock/Noagent.zh-CN.json
```json5
{
  "检测服务": "blackbox" // blackbox 或 vpndetection
}
```

## 更新日志

### v1.0.6
- 新增配置项「检测服务」，可改用 VPNDetection

### v1.0.0
- 添加插件


## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love