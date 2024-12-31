# MusicPlayer 简易音乐播放器

- 作者: Olink，Cjx适配，肝帝熙恩修改, yu大改
- 出处: [github](https://github.com/Olink/SongPlayer) 远古仓库
- 装上插件会在Tshock文件夹下生成一个Songs文件夹，里面放音乐文件
- 通过读取音高文件，使用竖琴/铃铛/吉他模拟播放音乐，也就是参数：harp，bell，theaxe
- 提供了[简易转换脚本](https://github.com/UnrealMultiple/TShockPlugin/blob/master/src/MusicPlayer/sample_converter.py)
- 和[便捷exe工具](https://github.com/UnrealMultiple/TShockPlugin/blob/master/src/MusicPlayer/sample_converter.exe)，将mid文件拖动到便捷exe上即可转换
- 你也可以自己手搓,这里有[示范音乐转换文件（mid），和成品文件](https://github.com/UnrealMultiple/TShockPlugin/tree/master/src/MusicPlayer/demo)
- 不推荐过高音和过低音，泰拉本身不支持，音高范围为C4-C6，转换的时候并不会对该范围内的音高进行改动，保留且使用时不播放外围的音高
- 只能播放非常简单的音乐，音轨越少越好，起伏越好，将常规mp3转换成mid再转换成本插件可用格式基本不可行，建议寻找专用的midi网站获取midi，或者自己扒谱

## 指令

| 语法                                 |    权限    |           说明            |
|------------------------------------|:--------:|:-----------------------:|
| /song [歌曲名字] [harp/bell/theaxe]    |   song   | 为自己播放/停止播放歌曲，默认参数为harp  |
| /songall [歌曲名字] [harp/bell/theaxe] | songall  | 为所有人播放/停止播放歌曲，默认参数为harp |
| /songlist                          | songlist |        查看歌曲文件列表         |

## 配置
> tshock/Song文件夹内放音乐文件
```
200//速度，越小越快
D#4
F4
F#4
G#4
A#4
0     //停顿用0
D#5
C#5
A#4
```

## 更新日志

```
v1.0.3
初始化时才创建目录
```

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
