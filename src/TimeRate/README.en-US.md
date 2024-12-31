# TimeRate

- Authors: Yu Xue
- Source: Tshock Official QQ Group 816771079
- This is a Tshock server plugin, primarily used for: modifying time acceleration using commands, and supporting player sleep to trigger events.

## Commands

| Syntax            |   Aliases   | Permission  |              Description               |
|-------------------|:-----------:|:-----------:|:--------------------------------------:|
| /times            |    /时间加速    | times.admin |        Time acceleration menu.         |
| /times all        |  /时间加速 all  | times.admin | Toggle All Players sleep acceleration. |
| /times one        |  /时间加速 one  | times.admin | Toggle One Player sleep acceleration.  |
| /times on         |  /时间加速 on   | times.admin |   Enable command time acceleration.    |
| /times off        |  /时间加速 off  | times.admin |   Disable command time acceleration.   |
| /times set number | /times s 数值 | times.admin |         Set acceleration rate.         |

---
Configuration Notes
---
1.`Command Acceleration` can be controlled with /times on and /times off.
2.When `Group Sleep Acceleration` is enabled, `Individual Sleep Acceleration` is disabled; upon waking up, the default flow rate resumes.
3.`Visual Smoothness Optimization` determines whether TimeSet data packets are sent to improve the effect of time flow.
4.All configuration items depend on the `Acceleration Rate`. The enchanted sundial flow rate is 60, and the normal rate is 1.

## Configuration
> Configuration file location：tshock/时间加速.json
```json5
{
  "Command Acceleration": false,
  "Group Sleep Acceleration": true,
  "Individual Sleep Acceleration": false,
  "Visual Smoothness Optimization": true,
  "Acceleration Rate": 180
}
```
## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love