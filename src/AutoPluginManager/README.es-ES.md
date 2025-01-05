# PluginManager

- 作者: 少司命，Cai，LaoSparrow
- 出处: 本仓库
- Usa el comando para actualizar automáticamente los plugins del servidor (solo desde este repositorio).
- 提供了热重载插件的可能，但此热重载并不算是真正意义上的热重载，已加载的程序集无法卸载，而是依然存在于应用域
  中，只是不再生效!

## Comandos

| 语法                                                                           |      Permiso     |                                                                                 说明                                                                                 |
| ---------------------------------------------------------------------------- | :--------------: | :----------------------------------------------------------------------------------------------------------------------------------------------------------------: |
| /apm -c                                                                      | AutoUpdatePlugin |                                                                Verificar actualizaciones de plugins                                                                |
| /apm -u [nombre plugin]  | AutoUpdatePlugin |  Actualizar plugins con un solo clic, requiere reinicio del servidor. Múltiples nombres de plugins se pueden separar por `comas`.  |
| /apm -l                                                                      | AutoUpdatePlugin |                                                               Ver la lista de plugins del repositorio                                                              |
| /apm -i [número plugin]  | AutoUpdatePlugin | Instalar plugins, requiere reinicio del servidor. Múltiples números de plugins se pueden separar por `comas` y usar con `/apm -i`. |
| /apm -b [nombre plugin]  | AutoUpdatePlugin |                                                                  Excluir plugin de actualizaciones                                                                 |
| /apm -r                                                                      | AutoUpdatePlugin |                                                               Verificar plugins duplicados instalados                                                              |
| /apm -rb [nombre plugin] | AutoUpdatePlugin |                                                                Eliminar exclusión de actualizaciones                                                               |
| /apm -lb                                                                     | AutoUpdatePlugin |                                                             Listar plugins excluidos de actualizaciones                                                            |
| /apm -ib                                                                     | AutoUpdatePlugin |                                                                           列出已安装插件列表与启用状态                                                                           |
| /apm -on [序号]            | AutoUpdatePlugin |                                                                               启用某个插件                                                                               |
| /apm -off [序号]           | AutoUpdatePlugin |                                                                               关闭某个插件                                                                               |

## Configuración

> Ubicación del archivo de configuración: tshock/AutoPluginManager.json

```json5
{
  "允许自动更新插件 ": false, // Permitir actualizaciones automáticas de plugins
(Permitir la actualización automática de complementos)
  "使用Github源 ": true, // Usar fuente de GitHub
(Usando la fuente de Github)
  "使用自定义源 ": false, // Usar fuente personalizada
(Utilice fuentes personalizadas)
  "自定义源清单地址 (Dirección de lista de fuentes personalizada)": "", // URL del manifiesto de plugins de fuente personalizada
  "自定义源压缩文件地址 ": "", // URL del archivo comprimido de plugins de fuente personalizada
(Dirección de archivo comprimido de origen personalizado)
  "插件排除列表 ": [], // Lista de exclusión de plugins
(Lista de exclusión de complementos)
  "热重载升级插件 ": true, // Actualizar plugins con recarga en caliente
(Complemento de actualización de recarga en caliente)
  "热重载出错时继续 ": true // Continuar si ocurre un error durante la recarga en caliente
(La recarga en caliente continúa por error)
}
```

## 更新日志

```
v2.0.3.1
更新英文翻译
v2.0.2.9
添加指令
/apm il 查看本地插件列表
/apm on 启用某个插件
/apm off 关闭某个插件
v2.0.2.8
调整apm指令管理
v2.0.2.7
完全适配自建API
v2.0.2.6
使用流缓存插件包,抛弃Gitee，自建API
v2.0.2.4
回滚到 v2.0.2.2
v2.0.2.3
添加 LazyAPI 依赖
配置文件本地化
v2.0.2.2
默认启用热重载
新增配置项热重载出错时跳过报错插件
新增`HotReload`字段以跳过指定插件热重载
细化一些提示
v2.0.2.1
修复在未开启热重载时，插件更新后版本号未更新
修复英文翻译错误
v2.0.2.0
重构部分逻辑
实现自动拉取依赖
实现自定义源
v2.0.1.7
热重载升级插件
v2.0.1.4
添加英文翻译
v2.0.1.3
添加配置项可切换到Github源
v2.0.1.2
将源更换为Gitee
v2.0.1.1
更新: apm u支持排除插件,支持自动更新插件,apm l优化显示 & 修复: 插件更新回旧版本,更新插件后不重启仍提示更新
v2.0.0.3
修复: 使用插件包最新目录结构
v2.0.0.2
更新: 插件仓库链接
v2.0.0.1
补全卸载函数
V2.0.0.0
1.正式更名为AutoPluginManager
2.添加安装插件功能
3.更改指令使用方式
```

## 反馈

- Github Issue -> Repositorio de TShockPlugin: https://github.com/UnrealMultiple/TShockPlugin
- Grupo de QQ de TShock: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
