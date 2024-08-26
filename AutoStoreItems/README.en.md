# AutoStoreItems automatic storage

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!

- Author: feather science cmgy Kan
- Source: None
- Holding the specified items is automatically stored in the storage space according to the configured item ID.
- (Support automatic deposit, thanks to cmgy's coin stacking algorithm)
  
## Update log
```
v1.2.4
加入对装备饰品的检测
可以定义装备什么饰品触发自动储存
预设物品加入常见方块（非人工方块）

v1.2.3 
cmgy雱的钱币叠堆算法
修复自动存储超堆叠BUG
加入了对钱币计算的单独逻辑，储钱速度与物品分开
储物空间物品超过9999时默认会恢复9999（不再增加）
移除对给玩家免检堆叠权限的播报
加入大量内置物品，大概抄了wiki一半的物品ID进去：
除了方块/家具/武器/装备/饰品/服装/染料等非叠堆9999的材料

v1.2.2
优化代码，添加使用说明
移除了【控超堆叠】配置项
使用本插件会使堆叠超上限
需给玩家免检堆叠权限：tshock.ignore.itemstack

v1.2.1
将物品ID改为数组

v1.2.0
修复储存速度修改无效导致刷物品bug
补充使用说明：不建议放药水进虚空袋
加入手持“储存道具”才会触发自动存储的配置项
加入了“控超堆叠”避免物品超过9999

v1.1.0  
移除了 `物品前缀` 的配置项  
加入手持或所选物品不会被自动存储  
自存支持保险箱、护卫熔炉、虚空袋  
预设配置存储物品加入：  
“凝胶”、"坠落之星"、“礼物”、“礼袋”、“护卫奖章”  
加入存储回馈提示（避免玩家不知道东西去哪了）  
  
v1.0.0  
根据背包/存钱罐/配置表里存在的物品进行自动存储  
```
  
## order
```
暂无
```

---
Configuration considerations
---
1.`Whether to hold it?` need to be selected `Possession of goods` one of them will start the storage function, and the other will be closed: backpack.`Contains one of them.` it will start.
    
2.`Storage speed` don't fall below `60 frames`(Recommended `120`), otherwise manually.`continuously` quickly put in `Same item to storage space grid` can lead to objects `Double the number` 
    
3.`Item name` will be in use `/Reload` according to the instruction `Item ID` automatic writing,`Quantity of articles` in order to store the minimum quantity requirements

4.`Equipment ornaments` only detect `Equipment 3+ornaments 7`, and `Storage speed` irrelevant, equipment designation `Ornaments (armor)` as long as the player moves or attacks, it will trigger self-storage
    
5.`There is a BUG` collected items will be `Cancel the collection`(refers to `Void bag` the potions are `Stack into a box` risk)
    
## deploy
> Configuration file location: tshock/ autosave. json
```json
{
   "装备持有检测": true,
   "背包持有检测": true,
   "存物速度(背包)": 120.0,
   "存钱速度(背包)": 20.0,
   "使用说明1": "[是否手持] 需要选中 [持有物品] 其中1个才会启动存储功能，关闭则背包含有 其中1个就会启动",
   "使用说明2": "[存物速度] 不要低于60帧(推荐120)，否则手动 [连续] 快速放入 [同样物品到存储空间格子] 会导致物品数量翻倍",
   "使用说明3": "[物品名] 会在使用 [/Reload] 指令时根据 [物品ID] 自动写入，[物品数量] 为储存最低数量要求 ",
   "使用说明4": "[装备饰品] 只会检测装备3格+饰品7格，与[存物速度]等无关，装备指定饰品(盔甲)玩家只要移动或攻击就会触发自存 ",
   "使用说明5": "[存在BUG] 收藏的物品会被取消收藏(指虚空袋的药水堆叠进箱子的风险) ",
   "存钱罐": true,
   "保险箱": true,
   "虚空袋": true,
   "护卫熔炉": true,
   "储存物品提示": true,
   "是否手持(↓)": false,
   "持有物品": [
    87,
    346,
    3213,
    3813,
    4076,
    4131,
    5325
  ],
   "装备饰品": [
    5126
  ],
   "储存物品表": [
    {
       "物品名(不用写)": "凝胶",
       "物品数量": 20,
       "物品ID": [
        23
      ]
    },
    {
       "物品名(不用写)": "护卫奖章, 坠落之星, 礼袋, 礼物",
       "物品数量": 1,
       "物品ID": [
        3817,
        75,
        1774,
        1869
      ]
    }
  ]
}
```
## feedback
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.