# Randombroadcast random broadcast

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!

- Author: Yu Xue
- Source: None
- Send a broadcast content randomly (support instruction)
- Write for group friends 35117 to learn

## Update log
v1.0.3
Improve the uninstallation function

v1.0.2
Bundle `Trigger probability` change `int` integer
Remove `Number of hair` the random value of randomness (when you write, you can send it at the same time)
Join `General probability`(Automatically calculate all `Trigger probability` cure
  
v1.0.1
Replace the timer to `Game update` triggering (that is, no one in your service is not time to take time)
Bundle `Default interval` the unit is changed from minutes to [seconds]
The fastest can reach `0.1 seconds` trigger once, if [the same number of hair], you will press``Geometric superposition``Trigger frequency
join in `Whether to turn on trigger` configuration item
  
## Order
```
暂无
```

---
Configuration Note
---
1. 1.`General probability` will automatically according to all `Content table` library `Trigger probability` automatic calculation
  
2.`Content` the default / or.
  
3..`Number of hair` fill in the real number, for example, you want to send it at the same time `2 group message content`,`Coexist` write `2`,,
don't have only 2 groups of you `Number of hair` write `9999`,`Screen` brush you
  
4. 4..`Default interval` write an integer, if you write 0.1 `0.1 seconds` essence
  
## Configuration
> Configuration file location: TSHOCK/Random Broadcast.json
```json
{
   "使用说明": "【总概率】会根据 【触发概率】自动计算，【消息内容】含【/或.】的会当指令执行，【同发数量】会随机发多组内容",
   "开启插件": true,
   "同发数量": 1,
   "默认间隔/秒": 1.1,
   "是否开启触发概率": true,
   "总概率(自动更新)": 2,
   "内容表": [
    {
       "触发概率": 1,
       "消息内容": [
         ".time 7:30",
         "我又来啦" 
      ],
       "触发颜色": [
        255.0,
        234.0,
        115.0
      ]
    },
    {
       "触发概率": 1,
       "消息内容": [
         "/time 19:30",
         "我又走啦" 
      ],
       "触发颜色": [
        190.0,
        233.0,
        250.0
      ]
    }
  ]
}
```
## feedback
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love