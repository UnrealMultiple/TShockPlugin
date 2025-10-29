# 迷宫生成器

- 作者: Eustia
- 出处: 本仓库
- 在游戏世界任意位置创建指定大小的随机迷宫
- 支持一键重置和重建已有迷宫
- 查看迷宫的完整可行路径
- 可配置迷宫的墙块、背景墙、油漆颜色
- 加入迷宫挑战，自动计时并生成排行榜

## 待更新功能

| 功能        | 状态  |
|-----------|-----|
| 迷宫内随机生成宝箱 | 未完成 |
| 随机生成一些陷阱  | 未完成 |
| 迷宫形状新增(大概有用？) | 未完成 |
## 指令

| 语法 | 别名 | 权限 | 说明           |
|------|------|------|--------------|
| /maze help | 无 | maze.generate | 显示帮助信息       |
| /maze build <名称> [大小] | 无 | maze.generate | 生成指定大小的迷宫(迷宫边长=大小x单元格)    |
| /maze join <名称> | 无 | maze.generate | 加入迷宫游戏       |
| /maze leave | 无 | maze.generate | 退出当前迷宫游戏     |
| /maze list | 无 | maze.generate | 列出所有已保存的迷宫位置 |
| /maze rank [页码] | 无 | maze.generate | 查看排行榜        |
| /maze del <名称> | 无 | maze.admin | 删除位置和清除方块    |
| /maze path <名称> | 无 | maze.admin | 显示/隐藏迷宫路径    |
| /maze reset <名称> | 无 | maze.admin | 重置指定迷宫       |
| /maze pos <名称> <tl\|bl\|tr\|br> | 无 | maze.admin | 设置迷宫位置（左上角、右上角、左下角、右下角）       |
## 注意事项

- 传入的“迷宫大小”是以单元格（cell）为单位。实际占用的面积 = (迷宫边长 × 单元格大小)^2。
  请确保生成位置有足够空间并遵守最小/最大限制。

## 配置
> 配置文件位置：tshock/MazeGenerator/MazeConfig.zh-CN.json
```json5
{
  "默认迷宫大小": 30,
  "最小迷宫大小": 5,
  "最大迷宫大小": 60,
  "单元格大小": 5,
  "迷宫墙壁图格ID": 267,
  "背景墙壁ID": 155,
  "背景油漆ID": 25,
  "路径显示油漆ID": 13,
  "游戏区域边界检查范围": 50,
  "排行榜显示每页记录数": 10
}
```

### 数据存储
- `positions.json` - 点位和对齐方式
- `sessions.json` - 迷宫会话数据
- `leaderboard.json` - 排行榜数据

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
