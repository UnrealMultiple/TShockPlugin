# Platform Determine player equipment

- author: Cai、cc04
- source: [github](https://github.com/ACaiCat/CaiPlugins)
- Determine what device the player uses to come in (only the type can be determined)

> [!NOTE]
> This plugin is used as a pre-plugin for other plugins to call (can be used for chat prefix, scoreboard determination, etc.)
> Currently supports differentiated PE, Stadia, XBOX, PSN, Editor, Switch, PC  

## Chat Prefix
You can use placeholders `%platform%`/`%device%` in the `ChatFormat` field of `tshock/config.json` to add device prefix translations.
```json5
"ChatFormat": "[%platform%]{1}{2}{3}: {4}", // eg: [PC]Cai: I love kotlin more than koko
"ChatAboveHeadsFormat": "{1}{2}{3}", //No support
"EnableChatAboveHeads": false, //No support, keep it false
```

## Configuration

| Syntax               |  Permission  |      Description      |
|----------------------|:------------:|:---------------------:|
| `/platform <player>` | platform.use | check player's device |

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
