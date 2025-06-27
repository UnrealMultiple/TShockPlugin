# CaiCustomEmojiCommand 自定义表情命令

- 作者: Cai
- 出处: 本仓库
- 懒得输一些长长的命令?试试自定义表情命令，将常用的命令转换为表情执行
- EmojiID可以在wiki(https://terraria.wiki.gg/zh/wiki/%E8%A1%A8%E6%83%85)上查询, 本插件不支持跳过权限检查, 命令需要标识符(/或者.)


## 指令
```
无
```
## 配置
> 配置文件位置：tshock/CaiCustomEmojiCommand.zh-CN.json
```json5
{
  "命令列表": [
    {
      "命令": "/who",
      "表情ID": 0
    },
    {
      "命令": "/home",
      "表情ID": 2
    },
    {
      "命令": "/back",
      "表情ID": 1
    },
    {
      "命令": "/进度补给",
      "表情ID": 149
    },
    {
      "命令": "/进度查询",
      "表情ID": 14
    },
    {
      "命令": "/进度列表",
      "表情ID": 11
    },
    {
      "命令": "/atp",
      "表情ID": 17
    } ,
    {
      "命令": "/vm",
      "表情ID": 90
    },
    {
      "命令": "/key",
      "表情ID": 7
    },
    {
      "命令": "/su",
      "表情ID": 3
    }
  ]
}
```
## 更新日志

### 2024.12.18.2 
- 准备更新TS 5.2.1
- 修正文档
### 2024.11.29.1 
- 使用lazyapi
### 2024.9.8.1 
- 添加英文翻译
### 2024.7.28.1 
- 添加插件


## 反馈

- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
