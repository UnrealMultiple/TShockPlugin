# ProgressBag Progress bag

- author: 少司命
- source: 无
- Whenever you complete a progress, you can receive a bag

## Instruction

| Command            |    Permissions     |    Description     |
|---------------|:---------:|:---------:|
| /bag r [bag name] |  bag.use  |   Receive bag    |
| /bag rall      |  bag.use  | Receive all available bag |
| /bag list      |  bag.use  |  View bag list   |
| /bag reset        | bag.admin |  Reset to receive bag   |

## Please check the ProgressType.cs file for specific progress.

## Configuration
> Configuration file location: tshock/ProgressBag.en-US.json
```json5
{
  "Bags": [
    {
      "Name": "无限制礼包",
      "ProgressLimit": [
        "无限制"
      ],
      "Award": [
        {
          "netID": 22,
          "prefix": 0,
          "stack": 99
        }
      ],
      "Commands": [],
      "ReceivePlayers": [],
      "Groups": []
    },
    {
      "Name": "克眼礼包",
      "ProgressLimit": [
        "克眼"
      ],
      "Award": [
        {
          "netID": 22,
          "prefix": 0,
          "stack": 99
        }
      ],
      "Commands": [],
      "ReceivePlayers": [],
      "Groups": []
    },
    {
      "Name": "史莱姆王礼包",
      "ProgressLimit": [
        "史莱姆王"
      ],
      "Award": [
        {
          "netID": 22,
          "prefix": 0,
          "stack": 99
        }
      ],
      "Commands": [],
      "ReceivePlayers": [],
      "Groups": []
    },
    {
      "Name": "世界吞噬者礼包",
      "ProgressLimit": [
        "世界吞噬者"
      ],
      "Award": [
        {
          "netID": 22,
          "prefix": 0,
          "stack": 99
        }
      ],
      "Commands": [],
      "ReceivePlayers": [],
      "Groups": []
    }
  ]
}
```

## Change log

```
v1.0.1.1
Add alias command bag
v1.0.1.0
Improve the uninstall function

```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
