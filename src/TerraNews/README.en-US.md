# TerraNews

- Author: MiMo
- Source: TShock plugin library
- Broadcasts the Angler's quest fish and tonight's moon phase at 04:30, plus sandstorm warnings and the Traveling Merchant's shelf
- The quest fish and the merchant's stock use vanilla **hoverable item icons** - details are one mouse-over away

## Notes

- 04:30 is **in-game time**; the trigger window defaults to 04:30-05:00 and is **never made up in the afternoon**
- The fish's name, biome, depth and how to hook it are **all inside the icon's hover tooltip**, so the chat line stays short
- The quest rolls on the same frame the broadcast fires (`Main.mfwh_AnglerQuestSwap`), so it is always the new task
- A dedicated server with **no players online does not advance the world**, so nothing is broadcast; use `/terranews` once you are in
- Sample output:

```
========== TerraNews · Angler's Quest ==========
Quest fish [i:2466]
Moon phase  Waxing Crescent
(In-game 04:30) use /terranews any time to see today's quest
```

## Commands

| Syntax | Aliases | Permission | Description |
|-------|:-------:|:----------:|:-----------:|
| /terranews | /news | everyone | Show today's quest fish and moon phase |
| /terranews broadcast | daily | tshock.admin | Broadcast today's board to the whole server now |
| /terranews storm | sandstorm weather | tshock.admin | Broadcast a weather warning now |
| /terranews merchant | shop | tshock.admin | Broadcast the merchant's shelf now |
| /terranews reload | reloadconfig | tshock.admin | Reload the config and reset the event detectors |

> Command aliases can be changed in the config's `CommandAliases`.

## Configuration
> Config file: tshock/TerraNews.json
```json5
{
  // Master switch. When off, every command except reload reports that it is disabled.
  "Enabled": true,
  // One switch per feature, all on by default.
  "Features": {
    "DailyQuestBoard": true,      // the whole 04:30 board
    "QuestFishIcon": true,        // the hoverable quest fish icon
    "FishingLocation": true,      // biome, depth, Y range, tip
    "AnglerStatus": true,         // whether the Angler is in town
    "MoonPhase": true,            // tonight's moon
    "Sandstorm": true,            // the onset of a sandstorm / blizzard
    "SandstormPeak": true,        // the follow-up when the storm peaks
    "TravelingMerchant": true,    // the merchant's shelf
    "ServerLog": true             // mirror every broadcast into the TShock log
  },
  "BroadcastHour": 4,             // in-game hour
  "BroadcastMinute": 30,          // in-game minute
  "TriggerWindowSeconds": 30,     // window width, in in-game minutes
  "StartupDelaySeconds": 5,       // quiet period after load
  "SandstormPeakSeverity": 0.95,  // severity that counts as "peak"
  "MerchantItemsPerLine": 5,      // icons per line, 0 = never wrap
  "NameSource": "both",           // fish name: zh | vanilla | both
  "Diagnostics": false,           // one line per second of trigger state
  "CommandPermission": "",        // empty = everyone
  "AdminPermission": "",          // empty = tshock.admin
  "CommandAliases": ["news"],
  "DailyLines": [
    "[c/4FC3F7:========== TerraNews · Angler's Quest ==========]",
    "[c/FFD966:Quest fish] [c/FFFFFF:{icon}]",
    "[c/B39DDB:Moon phase] [c/FFFFFF:{moon}]",
    "[c/888888:(In-game {time}) use /terranews any time to see today's quest]"
  ],
  "SandstormLines": [
    "[c/E0A458:========== TerraNews · Weather Warning ==========]",
    "[c/FFD966:{storm}]",
    "[c/FFFFFF:Severity {severity} · lasts about {remaining}]",
    "[c/888888:Sand over the desert, snow over the tundra - take care (In-game {time})]"
  ],
  "SandstormPeakLines": [
    "[c/E0A458:========== TerraNews · Weather Warning ==========]",
    "[c/FF6B6B:{storm} at its worst]",
    "[c/FFFFFF:Severity {severity} · lasts about {remaining}]",
    "[c/888888:Visibility is terrible, head back to town (In-game {time})]"
  ],
  "MerchantLines": [
    "[c/4FC3F7:========== TerraNews · The Merchant ==========]",
    "[c/FFD966:Today's stock (hover for details)]",
    "[c/FFFFFF:{items}]",
    "[c/888888:{count} items · while they last (In-game {time})]"
  ]
}
```

### Placeholders

- Daily: `{icon}` `{name}` `{name_zh}` `{name_en}` `{name_vanilla}` `{biome}` `{depth}`
  `{yrange}` `{tip}` `{angler}` `{time}` `{id}` `{moon}` `{moon_bonus}`
- Weather: `{storm}` `{severity}` `{remaining}` `{time}` `{moon}`
- Merchant: `{items}` `{count}` `{time}` `{moon}`

> The defaults are deliberately terse because the hover tooltip on `{icon}` already carries the name and the
> catching advice. **Every placeholder still works** - rewrite `DailyLines` to get the long board back, for example:
> ```json5
> "DailyLines": [
>   "[c/4FC3F7:========== TerraNews · Angler's Quest ==========]",
>   "[c/FFD966:Quest fish] [c/FFFFFF:{icon} {name}]",
>   "[c/FFD966:Where] [c/FFFFFF:{biome} · {depth}{yrange}]",
>   "[c/FFD966:Tip] [c/FFFFFF:{tip}]",
>   "[c/B39DDB:Moon phase] [c/FFFFFF:{moon} · {moon_bonus}]",
>   "[c/7FD4FF:{angler}]",
>   "[c/888888:(In-game {time}) use /terranews any time to see today's quest]"
> ]
> ```
> Note that `{angler}` must be plain text - it cannot carry its own colour tag.

### Switch behaviour

- Switching a feature off **removes every line that depends on it**, rather than leaving an empty label behind
- The default templates do not use `{biome}` `{depth}` `{tip}` `{angler}` `{moon_bonus}`, so with the shipped
  config `FishingLocation` and `AnglerStatus` have no visible effect until you rewrite `DailyLines`
- Invoking a disabled feature by hand tells you exactly which switch is off

## Changelog

### v1.3.0
- Moved to the repository's shared `GetString` i18n and added the `i18n` translation template
- Trimmed the plugin structure and dropped the abstraction layer that only existed to share code with the tModLoader build
- Folded four copies of the permission and feature-switch check into one

### v1.2.1
- The daily board is now 4 lines instead of 7: the quest fish is just the hoverable icon
- An untouched 1.2.0 default is swapped on upgrade; a hand-edited one is left alone

### v1.2.0
- Added the per-feature `Features` block, replacing 1.0's flat booleans
- Added `MerchantItemsPerLine` shelf wrapping

## Feedback

- Issues: https://github.com/UnrealMultiple/TShockPlugin/issues
- TShock group: 816771079
- TRHub: https://trhub.cn ; BBSTR: https://bbstr.net ; TR: https://tr.monika.love
