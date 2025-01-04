# DonotFuck

- Authors: 羽学 & Cai
- Source: [禁止脏话(内嵌版)](https://github.com/1242509682/DonotFuck)
- This is a Tshock server plugin primarily used for: detecting civilized speech among players on the server. 
- If a player sends characters listed in the configuration file, those words will be replaced with asterisks (*).


## Commands

| Command Syntax	 | Alias |    Permission     |               Description                |
|-----------------|:-----:|:-----------------:|:----------------------------------------:|
| /af             | None  |     DonotFuck     |               Command menu               |
| /af list        | None  |     DonotFuck     |    List sensitive words (for players)    |
| /af log         | None  |  DonotFuck.admin  | Enable or disable sensitive word logging |
| /af add <word>  | None  |  DonotFuck.admin  |           Add a sensitive word           |
| /af del <word>  | None  |  DonotFuck.admin  |         Remove a sensitive word          |
| /reload         | None  | tshock.cfg.reload |      Reload the configuration file       |

## Configuration
> Configuration file location： tshock/禁止脏话/禁止脏话.json
```json
{
  "RowsPerPage": 30,
  "LogEnabled": true,
  "SensitiveWords": [
    "6",
    "六"
  ]
}
```

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
