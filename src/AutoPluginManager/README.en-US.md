# AutoPluginManager Automatically update plugins

- author: 少司命，Cai，LaoSparrow
- source: 本仓库
- Use commands to automatically update server plugins (this repository only)
- Provides the possibility of hot reloading plugins, but this hot reloading is not a true hot reloading. The loaded assembly cannot be uninstalled, but still exists in the application domain, it just doesn’t work anymore!

## Instruction

| Command           |        Permissions         |   Description   |
| -------------- | :-----------------: | :------: |
| /apm c | AutoUpdatePlugin   | Check for plugin updates|
| /apm u [Plugin name] | AutoUpdatePlugin   | To update the plugin with one click, you need to restart the server. You can select multiple plugin names separated by English commas.|
| /apm l | AutoUpdatePlugin   | View the list of repository plugins |
| /apm i [list number] | AutoUpdatePlugin   | To install the plugin, you need to restart the server. Select the plugin list number separated by commas and use it with the `/apm -i` command. |
| /apm b [Plugin name] | AutoUpdatePlugin   | Exclude plugins from updates |
| /apm r | AutoUpdatePlugin   | Check for duplicate installed plugins |
| /apm rb [Plugin name] | AutoUpdatePlugin   | Remove excluded updates |
| /apm lb | AutoUpdatePlugin   | List plugins that are excluded from updates |
| /apm il | AutoUpdatePlugin   | List of installed plugins and their activation status |
| /apm on [list number] | AutoUpdatePlugin   | Enable a plugin |
| /apm off [list number] | AutoUpdatePlugin   | Close a plugin |

## Configuration

> Configuration file location: tshock/AutoPluginManager.json
```json5
{
  "允许自动更新插件": false, //Allow automatic update of plugins
  "插件排除列表": [], //Plugin exclusion list
  "热重载升级插件": true, //Hot reload upgrade plugin
  "热重载出错时继续": true //Hot reload continues on error
}
```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
