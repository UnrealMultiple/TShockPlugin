# HouseRegion 圈地插件

- 作者: GK Eustia更新
- 出处: 无
- 更好的圈地插件

## 指令

| 语法                        | 别名 |            权限             |     说明      |
|---------------------------|:--:|:-------------------------:|:-----------:|
| /house set 1              | 无  |         house.use         |    敲击左上角    |
| /house set 2              | 无  |         house.use         |    敲击右下角    |
| /house clear              | 无  |         house.use         |   重置临时敲击点   |
| /house allow [玩家] [房屋]    | 无  | `house.use` `house.admin` |    添加所有者    |
| /house disallow [玩家] [房屋] | 无  | `house.use` `house.admin` |    移除所有者    |
| /house adduser [玩家] [房屋]  | 无  | `house.use` `house.admin` |    添加使用者    |
| /house deluser [玩家] [房屋]  | 无  | `house.use` `house.admin` |    删除使用者    |
| /house delete [屋名]        | 无  | `house.use` `house.admin` |    删除房屋     |
| /house list [页码]          | 无  | `house.use` `house.admin` |   查看房屋列表    |
| /house redefine [屋名]      | 无  | `house.use` `house.admin` |   重新定义房屋    |
| /house info [屋名]          | 无  | `house.use` `house.admin` |    房屋信息     |
| /house lock [屋名]          | 无  | `house.use` `house.admin` |     房屋锁     |
| /house show [屋名]          | 无  |         house.use         |   房屋区域查看    |
| /house showall            | 无  |         house.use         |  查看所有房屋区域   |
| /house auto               | 无  |         house.use         | 进入区域后自动显示区域 |

## 配置
	配置文件位置：tshock/HouseRegion.json
> HouseRegion.json

```json
{
  "进出房屋提示": true,
  "房屋嘴大大小": 1000,
  "房屋最小宽度": 30,
  "房屋最小高度": 30,
  "房屋最大数量": 1,
  "禁止锁房屋": false,
  "保护宝石锁": false,
  "始终保护箱子": false,
  "冻结警告破坏者": true,
  "禁止分享所有者7": false,
  "禁止分享使用者": false,
  "禁止所有者修改使用者": true
}
```

## 更新日志
### v1.0.3
- 添加新功能-显示圈地区域

### v1.0.2
- 修正圈地消息提示错误的问题

### v1.0.1
- 修复草药不被保护的问题

### v1.0.0.4
- 完善卸载函数

## 反馈

- 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 国内社区 trhub.cn 或 TShock 官方群等
