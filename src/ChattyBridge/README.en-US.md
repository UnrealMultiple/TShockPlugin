# ChattyBridge

- author: 少司命
- source: none
- Allow multiple servers to chat with each other


## Instruction
```
none
```
## Configuration
> Configuration file location: tshock/ChattyBridge.en-US.json
```json5
{
  "forward_command": false,
  "rest_address": [],
  "server_name": "",
  "token": "", //The token is not a Rest token
  "message_option": {
    "message_format": "[{0}]{1}: {2}", //0 is the server name, 1 is the player name, 2 is the message
    "leave_format": "[{0}]{1}Leaves server",
    "join_format": "[{0}]{1}Join server"
  }
}
```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
