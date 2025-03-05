# AIChatPlugin AI聊天插件

- 它允许玩家通过聊天与一个 AI 对话系统进行互动
- 该插件提供了一个简单的接口，使得玩家可以通过特定的命令或聊天触发词来向 AI 提出问题，并接收回答。

## 命令

| 指令 | 权限 | 作用 |
| :-----| ----: | :----: |
| /ab | 无 | 向 AI 提问 |
| /bcz | 无 | 清除你的上下文记录 |
| /bbz | 无 | 显示帮助信息 |
| /aiclear | aiclear | 清除所有玩家的上下文记录 |
| /reload | tshock.cfg.reload | 重载配置文件 |

## 配置文件

> 配置文件位于tshock/AIChat.json

```
{
  "Answer the word limit": 666,
  "Answer the word count of word wraps": 50,
  "Contextual limitations": 10,
  "The AI answers the timeout": 100,
  "The name displayed when the AI answers": "AI",
  "Set up answer requirements": "You're a concise and effective multilingual AI that is adept at summarizing complex questions in one sentence in the language of the questioner"
}
```

## 注意事项

- 确保你的服务器网络连接正常，以便 AI 能够访问必要的 API。
- AI无法回答敏感话题，否则会报错

## 更新日志

```
v2025.3.5 优化api调用
v2025.1.13 修复执行/ab后玩家移动停顿问题与优化设定支持多语言，优化提问限制
```

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love