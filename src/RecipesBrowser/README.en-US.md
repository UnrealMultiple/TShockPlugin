# RecipesBrowser recipes table

- author: 棱镜、羽学
- source: [github](https://github.com/1242509682/RecipesBrowser)
- due to some vicious bugs in the guide of PE Terraria (ancient times)
- as a result, the wizard is disabled in most servers, which makes it very troublesome to look up the table.
- so I wrote a plug-in that supports searching for "the recipe of this item" and "what this item can be synthesized into."

## Instruction

| Command        |       Permissions       |            Description            |
|-----------|:--------------:|:------------------------:|
| /fd、/find | RecipesBrowser | /fd <Item ID> Query the materials and workstations required for synthesis  |
| /查        | RecipesBrowser | /fd <Item ID> Query the items that can be synthesized from this material |

## Configuration

```json
none
```

## Change log

```
- 1.0.6
- Improve the uninstall function
- 0.5
- Fixed an error when shutting down the server due to unreleased hooks
- 0.4
- Adapted to .net 6.0
- Added Chinese commands and added a permission name
```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
