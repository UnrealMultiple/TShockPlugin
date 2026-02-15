# GenerateMap 将地图保存至图片

- 作者: 少司命, Cai，千亦
- 出处: 无
- 生成地图图片

## 指令

| 语法        |     权限      |   说明   |
|-----------|:-----------:|:------:|
| /map img  | generatemap | 生成地图图片 |
| /map file | generatemap | 生成地图文件 |

## REST API

| 路径                |     权限      |      说明      |
|-------------------|:-----------:|:------------:|
| /generatemap/img  | generatemap | 获取地图图片base64 |
| /generatemap/file | generatemap | 获取地图文件base64 |

## 配置

```json5
暂无
```

## 更新日志

### v2.1.0
- 适配 1.4.5
- 修复在 Linux， macOS 下的保存路径错误

### v2.0.0
- 移除CaiLib依赖，重构

### v1.0.2
- 改介绍
### v1.0.1
- 完善rest卸载函数


## 反馈

- 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 国内社区 trhub.cn 或 TShock 官方群等
