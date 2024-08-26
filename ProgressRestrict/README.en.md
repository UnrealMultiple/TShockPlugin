# ProgressRestrict over-schedule detection

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Young Si Ming & Love magic change Trinity
- Source: github
- Over-schedule detection
- Can carry out over-schedule detection on items, barrage and buff.

> [! NOTE]
> Need to install the pre-plug-in: DataSync (this warehouse)

## Update log

```
暂无
```

## instruction

|grammar|limit of authority|explain|
| ---- |:------------------------:|:----------------------:|
|empty|progress.item.white|Permission to check the white name of over-scheduled items|
|empty|progress.projecttile.white|Super-schedule barrage check white name permission|
|empty|progress.buff.white|Super progress Buff check white name permission|

## deploy
> Configuration file location: tshock/ over-progress detection.json.
```json
{
   "惩罚违规": true,
   "惩罚Debuff时长": 5,
   "公示违规者": true,
   "写入日志": true,
   "清除违规物品": true,
   "清除违规状态": true,
   "踢出违规玩家": false,
   "限制列表": [
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "史莱姆王",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "克苏鲁之眼",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "邪恶Boss",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "蜂后",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "骷髅王",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "肉山",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "任一三王",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "双子魔眼",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "机械毁灭者",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "机械骷髅",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "世纪之花",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "石巨人",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "猪鲨公爵",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "拜月教徒",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "月球领主",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "日耀塔",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "星旋塔",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "星云塔",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "星尘塔",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "冰雪女王",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "圣诞坦克",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "永恒尖啸",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "哀嚎之木",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "史莱姆皇后",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "独眼巨鹿",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "光之女皇",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "血月",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "困难血月",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "日食",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "三王后日食",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "花后日食",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "南瓜月",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "霜月",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "火星暴乱",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "旧日军团",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "哥布林军团",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "困难哥布林军团",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "海盗军团",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "雪人军团",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "一阶旧日军团",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "二阶旧日军团",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "三阶旧日军团",
       "跨服解禁": false
    },
    {
       "限制物品": [],
       "限制弹幕": [],
       "限制状态": [],
       "对应进度": "不存在的进度",
       "跨服解禁": false
    }
  ]
}
```
## feedback
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.