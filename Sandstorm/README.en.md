# Sandstorm switch sandstorm

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!

- Author: Onusai Yu Xue
- - 出处: [TSHOCK-Onus-Utility](https://github.com/onusai/tshock-onus-utility) 
- Use the instruction to switch the dust and stop stopping. You can define the "wind speed value" in the configuration file

 ##Update log

```
1.0.1
完善卸载函数
```
## instruction

|grammar|Alias|Authority|illustrate|
| ------------------------------ |: ---:|: ----------------:|: ----------------------------------------:|
|/SD|/Sandstorm|sandstorm.admin|Switch sandstorm to open and stop|

## Configuration
> Configuration file location: TSHOCK/SANDSTORM.JSON
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
## feedback
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love