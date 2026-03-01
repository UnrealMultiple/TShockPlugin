# CGive 离线命令

- 作者: Leader，肝帝熙恩
- 出处: 无
- 离线执行命令，玩家登陆游戏时自动执行命令。

## Rest API

| 路径            | 权限 |      说明       |
|---------------|:--:|:-------------:|
| /getWarehouse | 无  | 查询/give命令具体信息 |

## 指令

| 语法                                   |     权限      |          说明           |
|--------------------------------------|:-----------:|:---------------------:|
| /cgive add [执行者] [被执行者] [命令]         | cgive.admin | 添加一条离线命令              |
| /cgive list                          | cgive.admin | 离线命令列表                |
| /cgive del [id]                      | cgive.admin | 删除指定id的离线命令           |
| /cgive reset                         | cgive.admin | 重置所有数据                |

### 参数说明

| 参数    | 可选值              | 说明                  |
|-------|------------------|---------------------|
| 执行者   | `Server` / 玩家名   | 以谁的身份执行命令 |
| 被执行者  | 玩家名 / `-1`       | 指定玩家，或 -1 表示所有玩家    |
| 命令    | 任意 TShock 命令     | 可用 `{name}` 作为目标名占位符  |

### 示例

```
# 玩家leader上线就kill她一次（leader 离线时保存，上线自动执行）
/cgive add server leader /kill {name}

# 任意玩家上线就kill他一次（包括未来登录的玩家，每人只一次）
/cgive add server -1 /give {name} 4956
```

## 配置

```json5
暂无
```

## 更新日志

### v1.0.1.1
- 修复 SQLite database is locked 错误
- 合并 personal/all 子命令为统一的 add 子命令，保留旧命令兼容
### v1.0.1.0
- 修复在MySQL下重置报错，修复ID不自增，允许不使用引号包围命令
### v1.0.0.8
- 修正 GetString
### v1.0.0.4
- i18n
- README_EN.md
### v1.0.0.3
- i18n预备
### v1.0.0.2
- 完善rest卸载函数
### V1.0.0.1
- 优化简化部分代码，完善卸载函数

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love