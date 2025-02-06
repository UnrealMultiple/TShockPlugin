# Economics.Task 任务插件

- 作者: 少司命
- 出处: 无
- 给玩家发任务

> [!NOTE]  
> 需要安装前置插件：EconomicsAPI、Economics.RPG (本仓库) 

## 指令

| 语法                |          权限          |    说明    |
|-------------------|:--------------------:|:--------:|
| /task pick [任务索引] |  economics.task.use  |  接取一个任务  |
| /task info [任务索引] |  economics.task.use  |  查看任务详细  |
| /task prog        |  economics.task.use  | 查看任务完成进度 |
| /task pr          |  economics.task.use  |   提交任务   |
| /task del         |  economics.task.use  |   放弃任务   |
| /task list [页码]   |  economics.task.use  |  查看任务列表  |
| /task reset       | economics.task.admin |   重置任务   |

## 配置
> 配置文件位置：tshock/Economics/Task.json
```json5
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
## 更新日志

```
v2.0.0.3
添加 GetString

V2.0.0.0
适配多货币
```
## 反馈

- 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 国内社区 trhub.cn 或 TShock 官方群等
