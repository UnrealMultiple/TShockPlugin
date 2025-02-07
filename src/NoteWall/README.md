# 留言墙 NoteWall

- 作者:肝帝熙恩
- 出处: 本仓库
- 玩家可以留下和查看留言。

## 指令

| 语法       | 别名                      | 权限                   | 说明                   |
|------------|---------------------------|------------------------|------------------------|
| /留言 <内容>      | addnote, add              | notewall.user.add      | 留下留言               |
| /查看留言 <序号/玩家名字>  | viewnote, vinote          | notewall.user.view     | 查看留言               |
| /留言墙 <页码>   | notewall                  | notewall.user.page     | 留言墙分页             |
| /随机留言   | randomnote, rdnote        | notewall.user.random   | 随机查看留言           |
| /修改留言 <序号>  | updatenote, upnote        | notewall.user.update   | 修改自己的留言         |
| /删除留言 <序号或玩家名字>  | deletenote, delnote       | notewall.admin.delete  | 删除留言（管理员权限），填玩家名字会删除该玩家所有留言 |
| /我的留言   | mynote                    | notewall.user.my       | 查看我的历史留言       |

## 配置

> 配置文件位置：tshock/NoteWall.zh-CN.json
```json5
暂无
```
> 数据库位置：tshock/tshock.sqlite/NoteWall


## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
