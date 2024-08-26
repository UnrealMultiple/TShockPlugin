# Economics.task task plug -in

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Shao Si Ming
- Source: None
- Send a task for players

> [! Note]
> Need to install front plug -ins: Economicsapi, Economics.rpg (this warehouse)

## Update log

```
暂无
```

## instruction

|grammar|Authority|illustrate|
| --------------------- |: ---------------------:|: ----------------:|
|/Task Pick [Mission Index]|economics.task.use|Take a task|
|/Task Info [Task Index]|economics.task.use|View the task details|
|/task prog|economics.task.use|View task completion progress|
|/Task PR|economics.task.use|Submit task|
|/task del|economics.task.use|Abandon the task|
|/task list [page number]|economics.task.use|View task list|
|/Task Reset|economics.task.admin|Reset|

## Configuration
> Configuration file location: TSHOCK/ECONOMICS/TASK.JSON
```json
{
   "不可重复接任务": true,
   "页显示数量": 10,
   "任务列表": [
    {
       "任务名称": "狄拉克的请求",
       "任务序号": 1,
       "前置任务": [],
       "限制接取等级": [],
       "限制接取进度": [],
       "任务内容": {
         "NPC对话": [17],
         "击杀怪物": [
          {
             "怪物ID": 2,
             "击杀数量": 2
          }
        ],
         "物品条件": [
          {
             "物品ID": 178,
             "数量": 10,
             "前缀": 0
          },
          {
             "物品ID": 38,
             "数量": 2,
             "前缀": 0
          }
        ]
      },
       "任务介绍": "哦，亲爱的朋友，你是来帮我的吗? 麻烦你去告诉商人那个老东西一声，让他不要忘记了我的生日，还有一件事最近有两只可恶的恶魔之眼，在我家附近，帮我杀掉他，并把晶状体给我，我还需要你去给我找几个红水晶，我要用这些打造一个神奇的小东西。作为报酬，我会请树妖对你进行赐福，在赠予你一些药水，它会让你更好的活下去。",
       "完成后提示": "哦，感谢你我的朋友,你叫{0}对吧，我记住了! 收好你的奖品!",
       "任务奖励": {
         "执行命令": ["/permabuff 165", "/i 499"]
      }
    }
  ]
}
```

## feedback

- Co -maintained plug -in library: https://github.com/Controllerdestiny/tshockplugin
- The official group of Trhub.cn or Tshock in the domestic community