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
| `/bl edit`   | `bosslock`  | 修改进度配置     |
| `/bl unlock` | `bosslock`  | 解锁进度       |
| `locklist`   | 无           | 进度列表       |

## 配置

> 配置文件位置：tshock/BossLock.json
```json5
{
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

### v2.0.0
- 部分重构，上传仓库

## 反馈

- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
