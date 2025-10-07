# BadApplePlayer - BadApple Video Player

- Author: Eustia
- Source: This repository
- Play BadApple video in Terraria
- 
> ⚠️ Warning: This plugin has only been tested in a single-player environment.  
> Although incremental rendering optimization and frame data compression have been implemented,  
> performance and smoothness in multi-player scenarios have not been verified and may result in lag or delays.

## Commands

| Syntax | Permission | Description |
|--------|------------|-------------|
| `/badapple help` | `badapple.use` | Show help information |
| `/badapple play <name> [loop]` | `badapple.use` | Play video (optional loop mode) |
| `/badapple pause <name>` | `badapple.use` | Pause playback |
| `/badapple resume <name>` | `badapple.use` | Resume playback |
| `/badapple stop <name>` | `badapple.use` | Stop playback |
| `/badapple list` | `badapple.use` | List all saved positions |
| `/badapple playing` | `badapple.use` | View currently playing sessions |
| `/badapple pos <name> <alignment>` | `badapple.admin` | Set playback position (alignment: tl/bl/tr/br) |
| `/badapple del <name>` | `badapple.admin` | Delete playback position (Admin only) |

## Usage

### 1. Set Playback Position
```
/badapple pos spawn br
```

### 2. Play Video
```
/badapple play spawn
```

## Feedback

- Priority: Open issue -> Collaborative plugin repository: https://github.com/UnrealMultiple/TShockPlugin
- Secondary: TShock official group: 816771079
- Less likely to be seen but also possible: Domestic communities trhub.cn, bbstr.net, tr.monika.love