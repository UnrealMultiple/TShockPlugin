# VotePlus

- Authors: Cai
- Source: None
- A multi-functional voting plugin


## Commands

| Command Syntax              | Alias |  Permission   |                 Description                 |
|-----------------------------|:-----:|:-------------:|:-------------------------------------------:|
| /vote kick <player>         | None  |   vote.use    |      Vote to kick the specified player      |
| /vote ban <player>          | None  |   vote.use    |      Vote to ban the specified player       |
| /vote mute <player>         | None  |   vote.use    |      Vote to mute the specified player      |
| /vote clearboss <BOSS name> | None  |   vote.use    | Vote to clear the specified BOSS (no drops) |
| /vote clearevent            | None  |   vote.use    |     Vote to clear all events in the map     |
| /vote day                   | None  |   vote.use    |     Vote to change the time to daytime      |
| /vote night                 | None  |   vote.use    |        Change the time to nighttime         |
| /vote rain                  | None  |   vote.use    |         Vote to start or stop rain          |
| /vote topic <topic>         | None  |   vote.use    |              Free topic voting              |
| /vote clearkick <player>    | None  |   vote.use    |     Remove a player from the kick list      |
| /vote kicklist              | None  |   vote.use    | View the list of players in the kick state  |
| /agree                      |  同意   |   vote.use    |         Agree with the current vote         |
| /disagree                   |  反对   | 	  vote.use		 |       Disagree with the current vote        |


## Configuration
> Configuration file location：tshock/VotePlus.json
```json5
{
  "启用投票踢出": true, // Enable voting to kick players
  "踢出持续时间(秒)": 600, // Duration (in seconds) for which a player is kicked
  "启用投票封禁": false, // Enable voting to ban players
  "启用投票禁言": true, // Enable voting to mute players
  "启用投票清除BOSS": true, // Enable voting to clear bosses (without drops)
  "启用投票关闭事件": true, // Enable voting to clear all events in the map
  "启用投票修改时间": true, // Enable voting to change the in-game time
  "启用投票修改天气": true, // Enable voting to change the in-game weather
  "启用自由投票": true, // Enable free topic voting
  "最小投票人数": 3, // Minimum number of players required to start a vote
  "投票通过率": 60 // Percentage of votes needed to pass a vote
}
```

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love