# BadApplePlayer - BadApple播放器

- 作者: Eustia
- 出处: 本仓库
- 在 Terraria 中播放 BadApple 视频

> ⚠️ 注意：本插件仅在单人环境下测试过。虽然实现了增量渲染优化和帧数据压缩，但在多人环境下的性能和流畅度尚未经过验证，可能存在卡顿或延迟问题。

## 指令

| 语法                            | 权限             | 说明                       |
|-------------------------------|----------------|--------------------------|
| /badapple help                | badapple.use   | 显示帮助信息                   |
| /badapple play \<名称\> [loop]  | badapple.use   | 播放视频（可选循环模式）             |
| /badapple pause \<名称\>        | badapple.use   | 暂停播放                     |
| /badapple resume \<名称\>       | badapple.use   | 继续播放                     |
| /badapple stop \<名称\>         | badapple.use   | 停止播放                     |
| /badapple list                | badapple.use   | 列出所有保存的位置                |
| /badapple playing             | badapple.use   | 查看当前播放中的会话               |
| /badapple pos \<名称\> \<对齐方式\> | badapple.admin | 设置播放位置（对齐方式：tl/bl/tr/br） |
| /badapple del \<名称\>          | badapple.admin | 删除播放位置（管理员）              |

## 使用方式

### 1. 设置播放位置

```
/badapple pos 主城 br
```

### 2. 播放视频

```
/badapple play 主城
```
### v1.0.1
- 修复了重启后tile无法清除的问题
## 反馈

- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love