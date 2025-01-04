# PluginManager

- Autores: 少司命, Cai
- Fuente: Aquí
- Usa el comando para actualizar automáticamente los plugins del servidor (solo desde este repositorio).

## Comandos

| Comando                  |     Permiso      |                                                              Detalles                                                              |
|--------------------------|:----------------:|:----------------------------------------------------------------------------------------------------------------------------------:|
| /apm -c                  | AutoUpdatePlugin |                                                Verificar actualizaciones de plugins                                                |
| /apm -u [nombre plugin]  | AutoUpdatePlugin |  Actualizar plugins con un solo clic, requiere reinicio del servidor. Múltiples nombres de plugins se pueden separar por `comas`.  |
| /apm -l                  | AutoUpdatePlugin |                                              Ver la lista de plugins del repositorio                                               |
| /apm -i [número plugin]  | AutoUpdatePlugin | Instalar plugins, requiere reinicio del servidor. Múltiples números de plugins se pueden separar por `comas` y usar con `/apm -i`. |
| /apm -b [nombre plugin]  | AutoUpdatePlugin |                                                 Excluir plugin de actualizaciones                                                  |
| /apm -r                  | AutoUpdatePlugin |                                              Verificar plugins duplicados instalados                                               |
| /apm -rb [nombre plugin] | AutoUpdatePlugin |                                               Eliminar exclusión de actualizaciones                                                |
| /apm -lb                 | AutoUpdatePlugin |                                            Listar plugins excluidos de actualizaciones                                             |

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

## Soporte

- Github Issue -> Repositorio de TShockPlugin: https://github.com/UnrealMultiple/TShockPlugin
- Grupo de QQ de TShock: 816771079
- Foro de Terraria en China: trhub.cn, bbstr.net, tr.monika.love