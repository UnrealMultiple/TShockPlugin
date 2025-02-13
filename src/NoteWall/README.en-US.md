# NoteWall

- **Author**: 肝帝熙恩  
- **Source**: [This Repository](https://github.com/UnrealMultiple/TShockPlugin)  
- **Description**: Allows players to leave and view messages on a shared wall.  

## Commands

| Command                      | Aliases (Incl. Chinese)     | Permission                | Description                                                                 |
|------------------------------|-----------------------------|---------------------------|-----------------------------------------------------------------------------|
| `/addnote <content>`            | `addnote`     | `notewall.user.add`       | Leave a message on the wall.                                               |
| `/vinote <ID/PlayerName>`      | `viewnote` | `notewall.user.view`      | View a specific message by ID or all messages from a player.               |
| `/notewall <Page/help>`           | `notewall`         | `notewall.user.page`      | Browse messages with pagination.                                           |
| `/rdnote`                    | `randomnote` | `notewall.user.random`    | View a random message from the wall.                                       |
| `/upnate <ID> <new_content>`   | `updatenote` | `notewall.user.update`    | Edit your own message by ID.                                               |
| `/delnote <ID/PlayerName>`    | `deletenote` | `notewall.admin.delete`   | Delete a message by ID or all messages from a player (Admin only).         |
| `/mynotes`                   | `mynote`         | `notewall.user.my`        | View all messages you've left.                                             |


## Configuration  
> Configuration file location：`tshock/NoteWall.en-US.json`  
```json5
{
  "MaxNotesperPlayer": 5,
  "MaxNoteLength": 50,
  "BannedWordsList": ""
}
```
> Database file location: `tshock/tshock.sqlite/NoteWall`  

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
