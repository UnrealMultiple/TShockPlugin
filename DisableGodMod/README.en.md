# Disablegodmod to prevent the player invincible

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: GK, Yu Xue
- Source: QQ group (232109072)
- The main purpose of the plug -in is to monitor and prevent players from using invincible or other cheating in the game.
- The changes in information such as the health, defense value, stealth state, dodge state, and the treatment effect of the player.
- The kick operation will be performed for illegal players. At the same time, in order to avoid concurrent problems,
- Synchronized lock processing of the player's data container.
- Analyze and judge the behavior of the player. When the suspected invincible or illegally modify the maximum life value of life, etc.,
## Update log

```
- 给插件添加了一个权限名，  
- 避免它对服主、超管等用户组也会实施同样的检测惩罚，  
- 并注释掉了它原本可能的封禁手段，带来的玩家不满。  
```
## instruction

|grammar|Authority|illustrate|
| -------------- |: --------------------:|: -------:|
|none|Invincible inspection|Do not detect the plug -in|

## Configuration

```json
暂无
```
## feedback
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love