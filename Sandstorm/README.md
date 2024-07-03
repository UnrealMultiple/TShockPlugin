# Sandstorm 切换沙尘暴
- 作者: onusai 羽学
- 出处: [tshock-onus-utility](https://github.com/onusai/tshock-onus-utility)
- 使用指令切换沙尘暴开启停止，可在配置文件定义'风速值'

 
## 指令

| 语法                             | 别名  |       权限       |                   说明                   |
| ------------------------------ | :---: | :--------------: | :--------------------------------------: |
| /sd | /沙尘暴 |  sandstorm.admin  | 切换沙尘暴开启与停止 |

## 配置
> 配置文件位置：tshock/Sandstorm.json
```json
{
  "使用说明": "指令：/sd 或 /沙尘暴，权限：Sandstorm.admin ",
  "是否允许指令开启沙尘暴": true,
  "是否开启广播": true,
  "广播颜色": [
    255.0,
    234.0,
    115.0
  ],
  "开启沙尘暴的风速目标值": 35,
  "关闭沙尘暴的风速目标值": 20
}
```
## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/Controllerdestiny/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
