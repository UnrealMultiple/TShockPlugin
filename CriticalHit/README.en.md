# CriticalHit hits text prompts

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Authors: White,Stone·Free, Gan Di Xi 'en,
- Source: TShock official group chat
- If crit is generated when attacking NPC, special floating text will pop up (the same as damage number).
- Different characters are generated according to different types of weapons.
- You can set to generate text even without crit.

## Update log

```
暂无
```

## instruction

```
暂无
```

## deploy
> Configuration file location: tshock/CriticalConfig.json
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
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.