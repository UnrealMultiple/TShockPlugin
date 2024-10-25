# PluginManager

- Authors: 少司命,Cai
- Source: Here
- Use the command to automatically update the server’s plugins (only from this repository).

## Commands

| Command                 |        Permission         |                                                               Details                                                                |
|-------------------------| :-----------------: |:------------------------------------------------------------------------------------------------------------------------------------:|
| /apm -c                 | AutoUpdatePlugin   |                                                      	Check for plugin updates                                                       |
| /apm -u [plugin name]   | AutoUpdatePlugin   |               	One-click upgrade plugins, requires server restart. Multiple plugin names can be separated by `commas`                |
| /apm -l                 | AutoUpdatePlugin   |                                                 	View the list of repository plugins                                                 |
| /apm -i [plugin number] | AutoUpdatePlugin   | 	Install plugins, requires server restart. Multiple plugin numbers can be separated by `commas` and used with the `/apm -i` command  |
| /apm -b [plugin name]   | AutoUpdatePlugin   |                                                     	Exclude plugin from updates                                                     |
| /apm -r                 | AutoUpdatePlugin   |                                                     	Check for duplicate installed plugins                                           |
| /apm -rb [plugin name]  | AutoUpdatePlugin   |                                                       Remove update exclusion                                                        |
| /apm -lb                | AutoUpdatePlugin   |                                                  List plugins excluded from updates                                                  |

## Config
> Configuration file location：tshock/Autoclear.json
```json
{
  "允许自动更新插件": false, // Allow automatic plugin updates
  "使用Github源": true, // Use Github source
  "插件排除列表": [], // Plugin exclusion list
  "热重载升级插件": true //Hot reload upgrade plugin
}
}
```

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
