# Switchcommand switch instruction

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Johuan, CJX, Yu Xue
- Source: None
- This is a TSHOCK server plug -in mainly used to give the instruction ability to "switch", which can ignore the execution methods of player permissions, custom cooling time, explanation of switching functions, etc.
- How to use:
- 1. Place the switch to place the wall instruction:/kg
- 2. Then use/kg adD god
- 3. Last/kg WC
- You will get an invincible instruction switch,
- If you need to ignore the permissions, you can send it at the second step:/kg qxhl
- The same is true of other instructions, ensuring that the last step is/kg WC

## Update log

```
1.2.3
完善卸载函数
1.2.2
修改中文变量为英文变量
添加权限，switch.ignoreperm,当有这个权限时，才可以赋予指令开关忽略权限的能力
1.2.1
汉化了配置文件，加入了新指令：/开关 说明
加入了只对指令开关的特殊保护，可通过配置项开启关闭
给每个独特的开关能返回自己的使用说明
对试图破坏指令开关的人进行反馈警告
```

## instruction

|grammar|Authority|illustrate|
| -------------- |: --------------------:|: -------:|
|/switch (switch) <Add/list/delete/cooling/explanation/permissions ignore/cancellation/rewind/complete>|switch.admin|Use switch instructions|
|/kg adD list del lq sm QXHL QX CB WC|switch.admin|Use switch instructions|
|/Switch iGnoreperm <True/False>|switch.ignoreperm|Independent permissions, when there is this permissions, can give the instruction switch to ignore the power of power|

## Configuration
> Configuration file location: TSHOCK/Switch Configuration Table.json
```json
{
   "是否开启开关保护": true,
   "试图破坏开关的警告": "你没有权限破坏指令开关！",
   "开关指令表": {
     "X: 1984, Y: 226": {
       "指令": [
         "/god" 
      ],
       "冷却时间": 0.0,
       "忽略权限": true,
       "开关说明": "我无敌了！！！" 
    },
     "X: 1985, Y: 226": {
       "指令": [
         "/butcher" 
      ],
       "冷却时间": 0.0,
       "忽略权限": false,
       "开关说明": "让我们清清怪~" 
    }
  }
}
```
## feedback
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love