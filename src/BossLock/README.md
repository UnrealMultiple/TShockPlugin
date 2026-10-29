# BossLock 进度锁

- 作者: Cai
- 出处: 本仓库
- 禁止召唤BOSS，控制进度


## 指令

| 语法           | 权限          | 说明         |
|--------------|-------------|------------|
| `/bl`        | `bosslock`  | 显示帮助信息     |
| `/bl list`   | `bosslock`  | 列出所有定时进度配置 |
| `/bl reset`  | `bosslock`  | 重置进度配置     |
| `/bl reset auto` | `bosslock` | 以配置的下一个北京时间为基准重置进度 |
| `/bl edit`   | `bosslock`  | 修改进度配置     |
| `/bl unlock` | `bosslock`  | 解锁进度       |
| `locklist`   | 无           | 进度列表       |

## 配置

> 配置文件位置：tshock/BossLock.json
```json5
{
  "AutoResetTimes": ["09:00", "21:00"], // 自动重置的北京时间，可修改，格式 HH:mm
  "Locks": [
    {
      "LockSeconds": 43200, // 解锁时间 (秒)，填-1不解锁
      "NpcId": 50,  // Boss的NPC ID
      "Name": "史莱姆王" // 自动生成，无需填写
    },
    {
      "LockSeconds": 43200,
      "NpcId": 4,
      "Name": "克苏鲁之眼"
    },
    {
      "LockSeconds": 43200,
      "NpcId": 668,
      "Name": "鹿角怪"
    }
  ]
}
```

## 更新日志

### v2.0.0.5
- 修复剩余时间为整分钟、整小时等情况时进度锁失效的问题，使用完整剩余时间判断。
- 修复拦截 Boss 生成时提示中的名称丢失。
- 新增 `/bl reset auto`，从 `AutoResetTimes` 选择下一个尚未到达的北京时间，默认早九点和晚九点。当天时刻全部经过时顺延到第二天；恰好到达某个时刻也选择下一个时刻。
- 自动重置仍按基准时间加各 Boss 的 `LockSeconds` 计算解锁时间，`-1` 永不解锁保持不变。服务器使用其他时区时会转换为服务器本地时间保存和显示。
- 旧配置缺少 `AutoResetTimes` 时使用默认值；空列表或非法时刻会拒绝自动重置，不清除已有进度。

### 自动重置配置

修改 `tshock/BossLock.json` 的 `AutoResetTimes` 后执行 `/reload`。例如 `["08:30", "20:30"]` 表示使用北京时间早上八点半和晚上八点半。列表可调整顺序，也可只设置一个时刻。

使用 AutoReset 时，在其配置的“重置后指令”列表中加入 `/bl reset auto`，即可在世界重置后更新进度基准；仅加载 BossLock 或重启游戏服不会自动清空进度。手动 `/bl reset` 和 `/bl reset yyyy-MM-dd-HH:mm:ss` 保持原行为。修改时刻配置不会重新计算已有进度，下一次执行自动重置时生效。

### v2.0.0.1
- 支持动态识别配置路径

### v2.0.0
- 部分重构，上传仓库

## 反馈

- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
