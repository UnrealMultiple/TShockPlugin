# yaaiomni
[![Total Downloads](https://img.shields.io/github/downloads/sgkoishi/yaaiomni/total?label=Downloads%40Release&style=for-the-badge) ![Latest Downloads](https://img.shields.io/github/downloads-pre/sgkoishi/yaaiomni/latest/total?label=Downloads%40Latest%20Release&style=for-the-badge)](https://github.com/sgkoishi/yaaiomni/releases) [![NuGet](https://img.shields.io/nuget/dt/Chireiden.TShock.Omni?label=NuGet&style=for-the-badge)](https://www.nuget.org/packages/Chireiden.TShock.Omni/)

Yet another misc plugin for TShock, collection of fixes, enhancements, utilities, debug commands, and more.

> __Note__
> If you are using Linux and don't know which version to download, download the tarball.

### Commonly used
* `/whynot` to show recent permission queries related to the player. The ultimate solution for all "what permission" questions.
* `/setlang`, `/maxplayers` to set the language and max players.
* `/settimeout`, `/setinterval`, `/clearinterval`, `/showdelay` to automate commands based on a timer.
* `/runas` to run commands as another player.
* `/resetcharacter`, `/exportcharacter` to reset or export a character.
* Chat spam limit to 3 msg/5 sec, 5 msg/20 sec (`.Mitigation.ChatSpamRestrict` in config).

### More features

* `.PlayerWildcardFormat`: `/g zenith *all*`
* `.HideCommands` and `.StartupCommands` will hide commands or run on startup.
* `.Enhancements.AlternativeCommandSyntax` supports `/command1 ; command2 ; command3 ...` and `/command1 && command2 && command3 ...`.
* `.Mode.Vanilla.Enabled` will add permissions to players for vanilla game play experience.
* `.CommandRenames`: `{"Chireiden.TShock.Omni.Plugin.Command_PermissionCheck": ["whynot123", "whynot456"]}`

### Advanced options

Run `/genconfig` to generate a full version of the config file. Hidden entries will be shown (unchanged entries will be hidden on next launch/reload).

> __Warning__  
> **KEEP IT UNCHANGED. DO NOT TOUCH UNLESS YOU KNOW WHAT YOU ARE DOING**

You will have access to all hidden features and control how they work. Read the [comments in `Config.cs`](Core/Config.cs) for more details.

### Extra

The `Chireiden.TShock.Omni.Misc` plugin contains some random features and utilities.
* Restrict specific boss summon, team and pvp status based on the permission.
* `.LavaHandler` stops lava spamming. It does not prevent lava from spawning, but rather vacuums it after it might spawn.
* Commands like `/echo`, `/_pvp`, `/_team` etc. can be used in minigames with other plugins.