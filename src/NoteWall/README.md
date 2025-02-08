# 留言墙 NoteWall

- 作者:肝帝熙恩
- 出处: 本仓库
- 玩家可以留下和查看留言。

## 指令

| 语法       | 别名                      | 权限                   | 说明                   |
|------------|---------------------------|------------------------|------------------------|
| /addnote <内容>      | addnote              | notewall.user.add      | 留下留言               |
| /vinote <序号/玩家名字>  | viewnote       | notewall.user.view     | 查看留言               |
| /notewall <页码/help>   | notewall                  | notewall.user.page     | 留言墙分页             |
| /rdnote   | randomnote        | notewall.user.random   | 随机查看留言           |
| /upnote <序号> <新内容>  | updatenote       | notewall.user.update   | 修改自己的留言         |
| /delnote <序号/玩家名字>  | deletenote       | notewall.admin.delete  | 删除留言（管理员权限），填玩家名字会删除该玩家所有留言 |
| /mynote   | mynote                    | notewall.user.my       | 查看我的历史留言       |

## 配置

> 配置文件位置：tshock/NoteWall.zh-CN.json
```json5
{
  "个人最大留言数量": 5,
  "留言字数限制": 50,
  "屏蔽词列表": ""
}
```
> 数据库位置：tshock/tshock.sqlite/NoteWall

## 更新日志

```
v1.0.2
修正文本和del逻辑
```


## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
