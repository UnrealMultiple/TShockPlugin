# SimultaneousUseFix 解决卡双锤卡星旋机枪之类的问题

- 作者: 肝帝熙恩，恋恋
- 出处: 无
- 解决卡双锤卡星旋机枪之类的问题
- 因为一些检测原因，最后一个桶会被判定，所以加了免检测
> [!NOTE]  
> 需要前置：https://github.com/sgkoishi/yaaiomni/releases

## 指令

| 语法 |         权限         |   说明   |
|----|:------------------:|:------:|
| 无  | SimultaneousUseFix | 完全免检权限 |

## 配置
    配置文件位置：tshock/解决卡双锤卡星旋机枪之类的问题.json
```json
{
  "免检测物品列表": [
    205,
    206,
    207,
    1128
  ],
  "是否杀死": true,
  "是否上buff": false,
  "buff时长(s)": 60,
  "上什么buff": [
    163,
    149,
    23,
    164
  ],
  "是否踢出": false
}
```


## 更新日志

```
v1.0.6
完善卸载函数
```

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
