# ExplosionGuard 爆炸守卫

非玩家来源的炮弹 / 爆炸（怪物、机关、环境等）保留伤害，但不再破坏方块（地形与建筑），
防止刷怪和爆炸把地图炸烂；玩家自己的爆炸不受影响。

## 功能
- 拦截非玩家炮弹的方块破坏（`Projectile.ExplodeTiles`）：保留伤害，只去掉拆方块。
- 白名单 `AlwaysAllowIds`：这些炮弹 ID 始终允许破坏（不拦截）。
- 黑名单 `AlwaysBlockIds`：这些炮弹 ID 始终禁止破坏（强制拦截）。
- 用 MonoMod 在运行时按方法名绑定，绑定失败自动降级（仅提示、不误伤）。

## 命令
| 命令 | 权限 | 说明 |
| --- | --- | --- |
| `/eg` | `eginfo` | 查看当前状态 |
| `/egreload` | `explosionguard.reload` | 热重载配置 |

## 配置 `tshock/爆炸守卫.json`
| 字段 | 说明 |
| --- | --- |
| `Enabled` | 总开关 |
| `AlwaysAllowIds` | 始终允许破坏的炮弹 ID 列表 |
| `AlwaysBlockIds` | 始终禁止破坏的炮弹 ID 列表 |

## 部署
TShock 6.1.0+，把 `ExplosionGuard.dll` 放进 `ServerPlugins/`，重启或用插件管理器加载。
