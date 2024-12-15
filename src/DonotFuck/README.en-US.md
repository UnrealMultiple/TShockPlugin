# DonotFuck

- Authors: 羽学 & Cai
- Source: [禁止脏话(内嵌版)](https://github.com/1242509682/DonotFuck)
- This is a Tshock server plugin primarily used for: detecting civilized speech among players on the server. 
- If a player sends characters listed in the configuration file, those words will be replaced with asterisks (*).

## Update Log

```
v3.1
- Added i18n English translation
- Added handling to determine if the message is a command
- Added logging of sensitive words to the `Swear Record` file under the configuration directory
- Added `/df` series commands
- Divided command permissions
- `DonotFuck`: Used for players to query sensitive words
- `DonotFuck.admin`: Used for administrators to add or remove sensitive words

v3.0
- Refactored code logic, removed banning logic. When a player uses a swear word, it will be replaced with asterisks (*).
- Changed the permission name for exempted players to `DonotFuck`.

v2.0
1. Fixed the issue where the count was not reset after a player was banned
2. Fixed the issue where sending any character once would result in a ban
3. Fixed the yellow chat spam caused by null reference exceptions
4. Fixed the issue where each time a player sent a swear word, it would not be broadcasted
   - The triggered sensitive words are now listed and output to all players and the console
5. Added logging
6. Removed the ["Enable"] option in the Config

v1.0:  
1. Added the ability to customize the following via the configuration file: enable switch, words, ban duration, whether to ban, and check frequency
2. Added a Reload method for the configuration file

```

## Commands

| Command Syntax	                             | Alias  |       Permission       |                   Description                   |
| -------------------------------- | :---: | :--------------: | :--------------------------------------: |
| /af  |  None  |   DonotFuck    |    Command menu    |
| /af list  |  None  |   DonotFuck    |    List sensitive words (for players)   |
| /af log  |  None  |   DonotFuck.admin    |    Enable or disable sensitive word logging   |
| /af add <word>  | None |   DonotFuck.admin    |    Add a sensitive word   |
| /af del <word>  |  None |   DonotFuck.admin    |    Remove a sensitive word    |
| /reload  | None |   tshock.cfg.reload    |    Reload the configuration file    |

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
