# MusicPlayer Simple music player

- Author: Olink，Cjx适配，肝帝熙恩修改, yu大改
- Source: [github](https://github.com/Olink/SongPlayer) Old Repository
- Installing the plugin will generate a Songs folder under the Tshock folder, which will store music files.
- By reading the pitch file, use the harp/bell/guitar to simulate playing music, that is, the parameters: harp，bell，theaxe
- [Easy Conversion Script] is provided(https://github.com/UnrealMultiple/TShockPlugin/blob/master/src/MusicPlayer/sample_converter.py)
- and [convenient exe tool](https://github.com/UnrealMultiple/TShockPlugin/blob/master/src/MusicPlayer/sample_converter.exe)，Drag the mid file to the convenient exe to convert
- You can also rub it by hand, here are [Demonstration Music Conversion File (mid), and Finished File](https://github.com/UnrealMultiple/TShockPlugin/tree/master/src/MusicPlayer/demo)
- It is not recommended to have too high and too lows. Terraria itself does not support it, and the pitch range is C4-C6. The pitch within this range will not be changed during conversion. The peripheral pitch will not be played when using it.
- Only play very simple music. The fewer tracks, the better, the better the ups and downs. It is basically not feasible to convert regular mp3 into mid and then convert the available format of the plugin. It is recommended to find a dedicated midi website to get midi, or dig the score yourself.

## Instruction

| Command                                 |    Permissions    |           Description            |
|------------------------------------|:--------:|:-----------------------:|
| /song [song name] [harp/bell/theaxe]    |   song   | Play/stop playing songs for yourself, the default parameter is harp  |
| /songall [song name] [harp/bell/theaxe] | songall  | Play/stop playing songs for everyone, default parameter is harp |
| /songlist                          | songlist |        View song file list         |

## Configuration
> tshock/Song文件夹内放音乐文件
```
200 //Speed, the smaller the faster
D#4
F4
F#4
G#4
A#4
0   //Use 0 for pause
D#5
C#5
A#4
```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love￼Enter
