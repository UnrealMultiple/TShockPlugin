# TrCDK CDK系统插件
- 作者: Jonesn
- 出处: ARK服务器
- 可以返回上次玩家的死亡地点，可自定义冷却时间
- 管理员能创建、删除、更新 CDK，玩家可使用有效 CDK 兑换（执行指令）奖励。
- CDK可以设置使用次数、过期时间、组限制、玩家限制。

## 指令
| 语法                                                          | 权限                | 说明          |
|-------------------------------------------------------------|-------------------|-------------|
| /cdk use <CDK兑换码>                                           | cdk.use           | 玩家兑换 CDK 礼包 |
| /cdk list                                                   | cdk.admin.loadall | 显示所有 CDK 列表 |
| /cdk add `<CDK名称>` `<使用次数>` `<过期时间>` `<指令>` `[组限制]` `[玩家限制]` | cdk.admin.add     | 添加新 CDK     |
| /cdk del `<CDK名称>`                                          | cdk.admin.del     | 删除指定 CDK    |
| /cdk update `<CDK名称>` `<使用次数>` `<过期时间>` `<指令>` `[组限制]` `[玩家限制]`         | cdk.admin.update  | 更新 CDK 信息   |
| /cdk give `<玩家名>` `<指令列表>`                                  | cdk.admin.give    | 给玩家 CDK 奖励  |

### 注意事项
- **/cdkadd 和 /cdkupdate**：过期时间格式 `yyyy-MM-ddThh:mm`，如 `2024-12-31T23:59`。组和玩家限制用逗号分隔，不限制填 `none`。
- **/cdkgive**：指令列表用逗号分隔，如 `/give 4956 [plr] 1,/heal [plr]`。

## 配置
> 数据库文件位置：tshock/TrCDK.sqlite


## 更新日志

### v1.0.0.0
- 初始版本发布

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net