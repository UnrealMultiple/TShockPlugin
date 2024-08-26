# Criticalhit strike text prompt

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: White, Stone · Free, Liver Emperor Xien
- Source: TSHOCK official group chat
- If the NPC has a crit, a special floating text will pop up (the same as the damage number)
- Different texts are generated according to different types of weapon types
- You can set the text even if you do not criticize

## Update log

```
暂无
```

## instruction

```
暂无
```

## Configuration
> Configuration file location: TSHOCK/CRITICALCONFIG.JSON
```json
{
   "总开关": true,
   "仅暴击显示": true,
   "消息分类": {
     "近战": {
       "详细消息设置": {
         "砰!": [
          255,
          120,
          0
        ],
         "嘭!": [
          255,
          40,
          50
        ],
         "啪!": [
          255,
          255,
          0
        ],
         "噗通!": [
          255,
          0,
          0
        ]
      }
    },
     "爆炸": {
       "详细消息设置": {
         "Boom!": [
          255,
          0,
          0
        ],
         "轰隆!": [
          255,
          0,
          0
        ]
      }
    },
     "远程": {
       "详细消息设置": {
         "Biu biu!": [
          50,
          255,
          10
        ]
      }
    },
     "魔法": {
       "详细消息设置": {
         "啪嗒!": [
          10,
          50,
          255
        ],
         "嗖!": [
          0,
          150,
          255
        ],
         "Whoomph!": [
          0,
          200,
          255
        ]
      }
    }
  }
}
```
## feedback
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love