# PersonalPermission Player individual permissions

- author: Megghy，肝帝熙恩更新1449
- source: [github](https://github.com/Megghy/PersonalPermission)
- to give the player an individual permission instead of adding it to the user group
- For developers: You can use `<Player Object (TSPlayer)>.GetData<Group>("PersonalPermission");` to obtain a user group object containing independent permissions, and calling `.HasPermission` directly will also automatically determine the permissions.

## Instruction

| Command  |            Permissions            |      Description      |
|-----|:------------------------:|:------------:|
| /pp | personalpermission.admin | Add/delete permissions and other instructions for players |

## Configuration

```
none
```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
