# AutoReset 完全自动重置插件

- 作者: cc04 & Leader & 棱镜 & Cai & 肝帝熙恩
- 完全自动重置插件,自定义要重置什么

## 指令

| 语法           |     权限      |  说明  |
|--------------|:-----------:|:----:|
| /reset <种子>	 | reset.admin | 重置世界 |
| /resetdata	  | reset.admin | 重置数据 |
| /rs	         | reset.admin | 重置设置 |


## 配置
> 配置文件位置: tshock/AutoReset/AutoReset.zh-CN.json
> 替换文件夹: tshock/AutoReset/ReplaceFiles
```json5
{
  "替换文件": {
    "/tshock/原神.json": "原神.json",
    "/tshock/XSB数据缓存.json": "" //表示删除/tshock/XSB数据缓存.json
  },
  "击杀重置": {
    "击杀重置开关": false,
    "已击杀次数": 0,
    "生物ID": 50,
    "需要击杀次数": 50
  },
  "重置后指令": [
    "/reload",
    "/初始化进度补给箱",
    "/rpg reset"
  ],
  "重置前指令": [
    "/结算金币"
  ],
  "重置后SQL命令": [
    "DELETE FROM tsCharacter"
  ],
  "地图预设": {
    "地图名": null,
    "地图种子": "",
    "特殊种子": 0
  }
}
```
## 更新日志

### v2026.2.12.0
- 适配1.4.5
- 重置时支持传入种子
- 支持彩蛋种子

### v2024.6.23.0 
- 移除CaiAPI重置提示
- 重置后清除世界事件
### v2024.12.8.1 
- 修复配置文件位置错误
- 在世界名称显示进度
### v2024.9.1 
- 添加英文翻译
- 添加`/resetdata`以重置数据不生成地图
### v2024.8.24 
- 尝试完善卸载函数

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
