# Economics.Task task plug-in

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Shao Siming
- Source: None
- Send tasks to players

> [! NOTE]
> Pre-plug-ins need to be installed: EconomicsAPI, Economics.RPG (this warehouse).

## Update log

```
暂无
```

## instruction

|grammar|limit of authority|explain|
| --------------------- |:------------------:|:--------------:|
|/task pick [task index]|economics.task.use|Pick up a task|
|/task info [task index]|economics.task.use|View task details|
|/task prog|economics.task.use|View the task completion progress|
|/task pr|economics.task.use|Submit a task|
|/task del|economics.task.use|Give up the task|
|/任务列表[页码]|经济.任务.用途|查看任务列表|
|/任务重置|经济.任务.管理|重置任务|

## 配置
> 配置文件位置:tshock/Economics/Task.json
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

## 反馈

- 共同维护的插件库:https://github . com/controller destiny/TShockPlugin
- 国内社区trhub.cn或TShock官方群等