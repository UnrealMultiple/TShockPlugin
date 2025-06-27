# TeleportRequest 传送请求

- 作者: MarioE, 修改者: Dr.Toxic，肝帝熙恩
- 出处: [github](https://github.com/MarioE/TeleportRequest)
- 传送前需要被传送者接受或拒绝请求，也就是`tpa`

## 指令
| 语法                             |         权限         | 说明                 |
|--------------------------------|:------------------:|:-------------------|
| `/atp` 或 `/接受tp`               | `tprequest.gettpr` | 接受传送请求             |
| `/autodeny` 或 `/自动拒绝tp`        | `tprequest.tpauto` | 自动拒绝所有人的传送请求       |
| `/autoaccept` 或 `/自动接受tp`      | `tprequest.tpauto` | 自动接受所有人的传送请求       |
| `/dtp` 或 `/拒绝tp`               | `tprequest.gettpr` | 拒绝传送请求             |
| `/tpahere <玩家>` 或 `/召唤玩家 <玩家>` |  `tprequest.tpat`  | 发出把指定玩家传送到你当前位置的请求 |
| `/tpa <玩家>` 或 `/传送至玩家 <玩家>`    |  `tprequest.tpat`  | 发出传送到指定玩家当前位置的请求   |


## 配置
> 配置文件位置：tshock/tpconfig.json
```json5
{
  "间隔秒数": 3,
  "超时次数": 3
}
```

## 更新日志

### v1.0.2 
- 添加英文翻译

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
