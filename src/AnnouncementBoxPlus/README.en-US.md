# AnnouncementBoxPlus

- Author: Cai
- Source: This repository
- Broadcast and management control

## Introduce

1. Broadcast box interpolated placeholders and formatting
2. You can add permissions to edit the broadcast box
3. Can switch the broadcast box   
4. The effective range of the broadcast box (unit: pixels) can be set


## Configuration
> Configuration file location: tshock/AnnouncementBoxPlus.json
```json5
{
  "禁用广播盒": false, //Disable the broadcast box
  "广播内容仅触发者可见": true, //Broadcast content is visible only by the trigger
  "广播范围(像素)(0为无限制)": 50, //Broadcast range (pixel) (0 is unlimited)
  "启用广播盒权限(AnnouncementBoxPlus.Edit)": true, //Enable broadcast box permissions(AnnouncementBoxPlus.Edit)
  "启用插件广播盒发送格式": false, //Enable plug-in broadcast box sending format
  "广播盒发送格式": "%当前时间% %玩家组名% %玩家名%:%内容% #详细可查阅文档", //Broadcast box sending format #Read more for details
  "启用广播盒占位符(详细查看文档)": true //Enable broadcast box placeholder (see the document in detail)
}
```
## Instruction

| Command     |   Description   |
|---------|:------:|
| /reload | Reload configuration files |

## Broadcast box format placeholder

| Placeholder    |   Description   |
|--------|:------:|
| %玩家组名% | The name of the player group |
| %玩家名%  |  Player name  |
| %当前时间% | Current real time |
| %内容%   | Original content of the broadcast box |

## Broadcast box content placeholder

| Placeholder         |      Description       |
|-------------|:-------------:|
| %玩家组名%      |    The name of the player group     |
| %玩家名%       |     Player name      |
| %当前时间%      |    Current real time     |
| %当前服务器在线人数% |  Get the current number of online users on the server  |
| %渔夫任务鱼名称%   |    Angler name    |
| %渔夫任务鱼ID%   |    Angler ID    |
| %渔夫任务鱼地点%   |    Angler location    |
| %地图名称%      |    Current map name     |
| %玩家血量%      |     Player's health      |
| %玩家魔力%      |     Player mana      |
| %玩家血量最大值%   |    Maximum player's health    |
| %玩家魔力最大值%   |    Maximum player mana    |
| %玩家幸运值%     |     Player Luck Value     |
| %玩家X坐标%     |    Player grid X coordinate    |
| %玩家Y坐标%     |    Player grid Y coordinates    |
| %玩家所处区域%    | Region of the player  |
| %玩家死亡状态%    |    Player's death status     |
| %当前环境%      |    Player's current environment     |
| %服务器在线列表%   | Online server list (/who) |
| %渔夫任务鱼完成%   |  Whether the player completes angler quest  |

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
