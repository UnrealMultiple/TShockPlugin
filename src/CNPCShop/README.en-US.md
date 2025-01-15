# CNPCShop Custom NPC Shop

- author: Megghy，肝帝熙恩更新1449
- source: [github](https://github.com/Megghy/CNPCShop)
- Allows you to customize the items sold by NPCs

## Instruction
```
none
```
## Configuration
> Configuration file location: tshock/CNPCShop.json
```json5
{
  "更新间隔": 500, //Update interval //The default is 500, in milliseconds. Generally speaking, this interval is almost enough, but if you want to replace it faster, you can lower it. However, it is recommended not to be lower than 100. Any lower will not only take up a lot of space Bandwidth is meaningless
  "总列表": [ //Total list
    {
      "启用": true, //Enable //Indicates whether to enable this shop block
      "世界ID[-1则不限制世界]": -1, //World ID [-1 means no world restriction] //Specify to take effect on a certain map. If you fill in -1, it will take effect in all worlds. The world ID can be viewed using /worldinfo
      "商店列表": [ //Shop list"
        {
          "启用": true, //Enable //Indicates whether to enable this store
          "用户组[留空则允许所有]": [ //User Groups [Leave blank to allow all]
            "guest",
            "default",
            "superadmin"
          ],   //Only user groups in this list can use custom shop
          "进入消息": [ //Enter message
            "Businessman: What do you want to buy?",
            "Businessman: Big sale!",
            "Businessman: Hello {name}"
          ],   //Text sent to the player when talking to an NPC. Multiple texts will be randomly selected. If left blank, they will not be sent. {name} in the text will be replaced by the player's name.
          "关闭消息": [ //Close message
            "Businessman: Welcome again"
          ],   //The text sent to the player when closing the conversation. Multiple texts will be randomly selected. If left blank, they will not be sent. {name} in the text will be replaced with the player's name.
          "NPCID": 17,   //The ID of the NPC to be replaced
          "商品": [ //Merchandise
            {
              "物品ID": 8, //Item ID //Item ID, see wiki for details
              "前缀": 0, //Prefix //item prefix, see wiki for details
              "价格": { //Price //Note that gold, silver, and copper can only be entered between 0-99, and platinum is 0-999.
                "铜": 99, //Copper
                "银": 3, //Silver
                "金": 0, //Gold
                "铂": 0 //Platinum
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
          "启用": false, //Enable
          "用户组[留空则允许所有]": [ //User Groups [Leave blank to allow all]
            ""
          ],
          "进入消息": [ //Enter message
            ""
          ],
          "关闭消息": [ //Close message
            ""
          ],
          "NPCID": 17,
          "商品": [ //merchandise
            {
              "物品ID": 1, //Item ID
              "前缀": 0, //Prefix
              "价格": { //Price
                "铜": 99, //Copper
                "银": 9, //Silver
                "金": 0, //Gold
                "铂": 0 //Platinum
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

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
