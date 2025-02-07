# NoteWall

- **Author**: 肝帝熙恩  
- **Source**: [This Repository](https://github.com/UnrealMultiple/TShockPlugin)  
- **Description**: Allows players to leave and view messages on a shared wall.  

---

## Commands

| Command                      | Aliases (Incl. Chinese)     | Permission                | Description                                                                 |
|------------------------------|-----------------------------|---------------------------|-----------------------------------------------------------------------------|
| `/addnote <content>`            | `留言`, `addnote`, `add`     | `notewall.user.add`       | Leave a message on the wall.                                               |
| `/vinote <ID/PlayerName>`      | `查看留言`, `viewnote`, `vinote` | `notewall.user.view`      | View a specific message by ID or all messages from a player.               |
| `/notewall <Page>`           | `留言墙`, `notewall`         | `notewall.user.page`      | Browse messages with pagination.                                           |
| `/rdnote`                    | `随机留言`, `randomnote`, `rdnote` | `notewall.user.random`    | View a random message from the wall.                                       |
| `/upnate <ID> <new_content>`   | `修改留言`, `updatenote`, `upnote` | `notewall.user.update`    | Edit your own message by ID.                                               |
| `/delnote <ID/PlayerName>`    | `删除留言`, `deletenote`, `delnote` | `notewall.admin.delete`   | Delete a message by ID or all messages from a player (Admin only).         |
| `/mynotes`                   | `我的留言`, `mynote`         | `notewall.user.my`        | View all messages you've left.                                             |

---

## Configuration  
> Configuration file location：`tshock/NoteWall.en-US.json`  
```json5
None
```
> Database file location: `tshock/tshock.sqlite/NoteWall`  

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
