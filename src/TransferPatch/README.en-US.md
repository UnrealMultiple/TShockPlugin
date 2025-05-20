# TransfersPatch

- Author: 少司命
- Source: This Repository
- Implement translation configuration classes by modifying the assembly
- He can translate most plugins
- For example, we can configure ts in Chinese
- This plugin provides default configuration translation TShockAPI

## Note
- The result of the modification of the plugin is permanent, and it is best to backup the plugin before using it.

## Instruction

```
None
```

## Configuration
> Configuration file location: tshock/TransferPatch.json
```json5
{
  "启用": true, //Enable
  "执行列表": [ //Execution list
    {
      "目标程序集": "ServerPlugins/TShockAPI.dll", //Target assembly
      "目标类": [ "TShockAPI.Configuration.TShockSettings", "TShockAPI.Configuration.ConfigFile`1" ], //Target class
      "翻译列表": { //Translation list
        "TShockAPI.Configuration.TShockSettings.ServerPassword": "Server Password",
        "TShockAPI.Configuration.ConfigFile`1.Settings":  "set up" //Generic class (`1` represents the number of generic parameters)
        "Config/Option": "Configuration" //Nested class (refers to the Option declaration in the Config class)
      }
    }
  ]
}
```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
