# SignInSign

+ Author: Soofa、羽学、少司命
+ Source: [github](https://github.com/Soof4/SignInSign)  
+ This is a Tshock server plugin mainly used for:
+ Players can use pop-up windows to display signs, and they can register and log in, obtain items, BUFF, teleport, record player passwords and other functions.
+ Notice:
+ Players who have installed this plugin will not be able to destroy all signs on the server unless they have sign.edit permissions.
+ Plugin will build a hidden sign at the spawn point based on the content of the configuration file.
+ To change the content of the sign:
+ 1.Use logged-in roles to dig out signs on the server
+ 2.Modify the "Create content of the sign" in the【告示牌登录.json】configuration file
+ 3.Enter the command: /gs r [Permissions: signinsign.setup]

## Instruction

| Command    |        Permissions        |        Description         |
|-------|:----------------:|:-----------------:|
| /gs r | signinsign.setup |       Reset the sign       |
| /gs s |  signinsign.tp   | Set up sign teleport points and automatically write configuration files |
| none     |    sign.edit     |     Allows to destroy sign permissions     |

## Configuration
> Configuration file location: tshock/告示牌登录.json
```json5
{
  "是否开启注册登录功能": true, //Enable the registration and login function
  "记录角色密码": false, //Record role password
  "对登录玩家显示告示牌": true, //Show signs to logged in players
  "是否允许点击告示牌": true, //Allowed to click on the sign
  "点击告示牌是否发广播": false, //Broadcast when click on the sign
  "创建告示牌的内容,重设指令:/gs r": "Welcome to the land reclamation server! ! \nThis server supports PE/PC cross-platform online play\nClean the world and the boss battle every 25 minutes\nFor more instructions, please enter /help\nClick the sign to teleport\n\nTShock official group: 816771079\n", //Create content of the sign, reset the command:/gs r
  "点击告示牌的广播/仅使用者可见": "Enter twice in this sign in sequence: \n[c/F7CCF0:123456] to register and log in.", //Click on the Broadcast of the Billboard/Visible only by users
  "试图破坏告示牌的广播": "This sign cannot be modified!", //Warning if trying to destroy the broadcast of the sign
  "点击告示牌执行什么指令": [], //Commands executed when clicking on the sign
  "点击告示牌给什么BUFF": [], //BUFF when clicked on the sign
  "点击告示牌BUFF时长/分钟": 10, //BUFF time/minute
  "点击告示牌给什么物品": [], //items give when clicking on the sign
  "点击告示牌给物品数量": 1, //Item amount given when click on the sign
  "点击告示牌是否传送,设置指令:/gs s": false, //Teleport when click on the sign, set the command: /gs s
  "点击告示牌传送坐标.X": 0.0, //Coordinates.X teleport when click the sign
  "点击告示牌传送坐标.Y": 0.0, //Coordinates.Y teleport when click the sign
  "点击告示牌传送特效": 10 //Special effects teleport when click on the sign
}
```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
