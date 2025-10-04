# TrCDK CDK 
- Author: Jonesn  
- Source: ARK Server  
- Can return the player's last death location with customizable cooldown time.  
- Administrators can create, delete, and update CDKs, and players can redeem (execute commands) rewards using valid CDKs.  
- CDKs can set usage times, expiration time, group restrictions, and player restrictions.


## Commands  
| Syntax | Permission | Description |  
| --- | --- | --- |  
| /cdk `<CDK Redeem Code>` | cdk.use | Players redeem CDK packages. |  
| /cdkloadall | cdk.admin.loadall | Display all CDK lists. |  
| /cdkadd `<CDK Name>` `<Usage Times>` `<Expiry Time>` `<Group Limit>` `<Player Limit>` `<Command>` | cdk.admin.add | Add a new CDK. |  
| /cdkdel `<CDK Name>` | cdk.admin.del | Delete the specified CDK. |  
| /cdkupdate `<CDK Name>` `<Usage Times>` `<Expiry Time>` `<Group Limit>` `<Player Limit>` `<Used Players>` `<Command>` | cdk.admin.update | Update CDK information. |  
| /cdkgive `<Player Name>` `<Command List>` | cdk.admin.give | Grant CDK rewards to a player. |  


### Notes  
- **/cdkadd and /cdkupdate**: Expiry time format is `yyyy-MM-ddThh:mm`, e.g., `2024-12-31T23:59`. Group and player limits are separated by commas; use `none` for no restrictions.  
- **/cdkgive**: Command lists are separated by commas, e.g., `/give 4956 [plr] 1,/heal [plr]`.  


## Configuration  
> Database file location: tshock/TrCDK.sqlite  


## Change Log  
### v1.0.0.0  
Initial release.  


## Feedback  
- Priority: Submit issues -> Shared plugin library: https://github.com/UnrealMultiple/TShockPlugin  
- Secondary: TShock Official Group: 816771079  
- Less frequently checked but acceptable: Chinese communities trhub.cn, bbstr.net