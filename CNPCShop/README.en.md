# CNPCShop custom NPC store

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Megghy, Gan Di Xi En Update 1449
- - 出处: [github](https://github.com/Megghy/CNPCShop) 
- Enables you to customize the goods sold by NPC.

## Update log

```
暂无
```

## instruction

without

## deploy
> Configuration file location: tshock/CNPCShop.json
```json
{
   "更新间隔": 500,  //默认为500, 单位毫秒, 一般来说这个间隔差不多够用了, 不过要是想替换得更快的话可以调低, 不过建议尽量不要低于100, 再低不仅十分占用带宽而且也没啥意义
   "总列表": [
    {
       "启用": true,   //表示是否启用此商店块
       "世界ID[-1则不限制世界]": -1,    //指定在某个地图生效, 填-1的话则在所有世界都生效. 世界id可使用/worldinfo 查看
       "商店列表": [
        {
           "启用": true,   //表示是否启用此商店
           "用户组[留空则允许所有]": [
             "guest",
             "default",
             "superadmin" 
          ],   //只有在此列表中的用户组才能使用自定义商店
           "进入消息": [
             "商人: 想买点啥?",
             "商人: 大甩卖!",
             "商人: 你好啊 {name}" 
          ],   //与npc进行对话时向玩家发送的文本, 多条文本会随机选择, 留空则不发送. 文本中的 {name} 将会被替换为玩家名字
           "关闭消息": [
             "商人: 欢迎再来" 
          ],   //关闭对话时向玩家发送的文本, 多条文本会随机选择, 留空则不发送. 文本中的 {name} 将会被替换为玩家名字
           "NPCID": 17,   //将要替换的npc的id
           "商品": [
            {
               "物品ID": 8,   //物品id, 详见wiki
               "前缀": 0,   //物品前缀, 详见wiki
               "价格": {   //注意 金银铜只能输入0-99之间, 铂为0-999
                 "铜": 99,
                 "银": 3,
                 "金": 0,
                 "铂": 0
              }
            },
            {
               "物品ID": 28,
               "前缀": 0,
               "价格": {
                 "铜": 99,
                 "银": 12,
                 "金": 0,
                 "铂": 0
              }
            },
            {
               "物品ID": 292,
               "前缀": 0,
               "价格": {
                 "铜": 99,
                 "银": 49,
                 "金": 0,
                 "铂": 0
              }
            },
            {
               "物品ID": 2350,
               "前缀": 0,
               "价格": {
                 "铜": 99,
                 "银": 99,
                 "金": 0,
                 "铂": 0
              }
            }
          ]
        },
        {
           "启用": false,
           "用户组[留空则允许所有]": [
             "" 
          ],
           "进入消息": [
             "" 
          ],
           "关闭消息": [
             "" 
          ],
           "NPCID": 17,
           "商品": [
            {
               "物品ID": 1,
               "前缀": 0,
               "价格": {
                 "铜": 99,
                 "银": 9,
                 "金": 0,
                 "铂": 0
              }
            },
            {
               "物品ID": 2,
               "前缀": 0,
               "价格": {
                 "铜": 5,
                 "银": 0,
                 "金": 0,
                 "铂": 0
              }
            }
          ]
        }
      ]
    }
  ]
}
```
## feedback
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.