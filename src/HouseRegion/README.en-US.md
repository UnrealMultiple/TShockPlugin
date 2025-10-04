# HouseRegion house protection plugin

- author: GK Eustia Updated
- Source: None
- House Protection Plugin

## instruction

| Command                        | Alias |            Permissions             |   Description    |
|---------------------------|:-----:|:-------------------------:|:-------:|
| /house set 1              | none  |         house.use         |  Tap the top left corner  |
| /house set 2              | none  |         house.use         |  Tap the bottom right corner  |
| /house clear              | none  |         house.use         | Reset temporary hit points |
| /house allow [player] [house]    | none  | `house.use` `house.admin` |  Add owner  |
| /house disallow [player] [house] | none  | `house.use` `house.admin` |  Remove owner  |
| /house adduser [player] [house]  | none  | `house.use` `house.admin` |  Add user  |
| /house deluser [player] [house]  | none  | `house.use` `house.admin` |  Delete user  |
| /house delete [house name]        | none  | `house.use` `house.admin` |  Delete house   |
| /house list [page number]          | none  | `house.use` `house.admin` | View house define list  |
| /house redefine [house name]      | none  | `house.use` `house.admin` | Redefining the house  |
| /house info [house name]          | none  | `house.use` `house.admin` |  Housing information   |
| /house lock [house name]          | none  | `house.use` `house.admin` |   House lock   |
| /house show [house]          | None  |         house.use         |  Show the boundary of a specific house  |
| /house showall               | None  |         house.use         |  Show the boundaries of all houses      |
| /house auto               | None  |         house.use         | Automatically display the area upon entering |
## Configuration
	Configuration file location: tshock/HouseRegion.en-US.json
> HouseRegion.json

```json5
{
  "JoinRegionText": true,
  "HouseMaxSize": 1000,
  "MinWidth": 30,
  "MinHeight": 30,
  "HouseMaxNumber": 1,
  "ProhibitLockHouse": false,
  "ProtectiveGemstoneLock": false,
  "ProtectiveChest": true,
  "WarningSpoiler": true,
  "ProhibitSharingOwner": false,
  "ProhibitSharingUser": false,
  "ProhibitOwnerModifyingUser": true
}
```

## Change log

```
v1.0.3
Added new feature - Display housing region boundaries

v1.0.2
Fixed incorrect message prompts when creating housing regions

v1.0.1
Fixed issue where herbs were not being protected

v1.0.0.4
Improve the uninstall function
```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
