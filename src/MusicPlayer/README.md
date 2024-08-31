# MusicPlayer 简易音乐播放器

- 作者: Olink，Cjx适配，肝帝熙恩修改, yu大改
- 出处: [github](https://github.com/Olink/SongPlayer) 远古仓库
- 装上插件会在Tshock文件夹下生成一个Songs文件夹，里面放音乐文件
- 通过读取音高文件，使用竖琴/铃铛/吉他模拟播放音乐
- 提供了[简易转换脚本](https://github.com/THEXN/TShockPlugin/blob/master/musicplayer/sample_converter.py)，你也可以自己手搓,和[示范音乐转换文件（mid），和成品文件](https://github.com/THEXN/TShockPlugin/tree/master/musicplayer/%E7%A4%BA%E4%BE%8B%E6%AD%8C%E6%9B%B2)
- 不推荐过高音和过低音，泰拉本身不支持，音高范围为C4-C6
- 
## 更新日志

```
暂无
```

## 指令

| 语法           |        权限         |   说明   |
| -------------- | :-----------------: | :------: |
| /song [歌曲名字] | song  | 为自己播放/停止播放歌曲 |
| /songall [歌曲名字] | songall   | 为所有人播放/停止播放歌曲 |
| /songlist [歌曲名字] | songlist   | 查看歌曲文件列表 |

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
## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
