# AIChatPlugin

- Allows players to interact with an AI dialogue system via chat
- The plugin provides a simple interface that allows players to ask questions to the AI ​​and receive answers via specific commands or chat triggers.

## Instruction

| Command | Permissions | Description |
| :-----| ----: | :----: |
| /ab | none | Ask the AI ​​questions |
| /bcz | none | Clear your context record |
| /bbz | none | Show help information |
| /aiclear | aiclear | Clear all player context records |
| /reload | tshock.cfg.reload | Reload configuration file |

## Configuration file

> Configuration file is located at tshock/AIChat.json

```
{
  "回答字限制": 666, //Answer word limit
  "回答换行字": 50, //Answer newline
  "上下文限制": 10, //context limit
  "超时时间": 100, //timeout
  "名字": "AI", //name
  "设定": "你是一个简洁高效的AI，擅长用一句话精准概括复杂问题。", //Setting": "You are a concise and efficient AI who is good at summarizing complex problems accurately in one sentence."
}
```

## Note

- Make sure your server has a good network connection so that the AI ​​can access the necessary APIs.
- AI cannot answer sensitive topics, otherwise it will report an error

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
