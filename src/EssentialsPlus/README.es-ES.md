# EssentialsPlus

- Autores: WhiteX y otros, Average, Cjx, adaptación y modificación por 肝帝熙恩, actualización por Cai
- Fuente: [Github](https://github.com/QuiCM/EssentialsPlus)
- Essentials+ es una combinación de funcionalidades de Essentials y MoreAdminCommands mejoradas. Todos los comandos se ejecutan de manera asíncrona. No incluye comandos de señales.所有命令都是异步执行的。不包括 Flag 命令。

## Comandos

- **/find** -> Acepta varios subcomandos:
  - **-command** -> Busca un comando específico basado en la entrada y devuelve los comandos coincidentes y sus permisos.
  - **-item** -> Busca un ítem específico basado en la entrada y devuelve los ítems coincidentes y sus IDs.
  - **-tile** -> Busca un bloque específico basado en la entrada y devuelve los bloques coincidentes y sus IDs.
  - **-wall** -> Busca una pared específica basada en la entrada y devuelve las paredes coincidentes y sus IDs.
- **/freezetime** -> Congela y descongela el tiempo.
- **/delhome** `<nombre_casa>` -> Elimina una casa específica con `<nombre_casa>`.
- **/sethome** `<nombre_casa>` -> Establece una casa con el nombre `<nombre_casa>`.
- **/myhome** `<nombre_casa>` -> Te teletransporta a tu casa llamada `<nombre_casa>`.
- **/kickall** 或 **/踢所有人** `<原因>` -> 踢出服务器上的所有人。**/kickall** `<flag> <razón>` -> Expulsa a todos los jugadores por `<razón>`. Bandera válida: `-nosave` -> La expulsión no guarda el inventario SSC.
- **/=** -> Repite el último comando ingresado (excluyendo otros usos de /=).
- **/more** 或 **/堆叠** -> 最大化手持物品的堆叠。子命令：
  - **-all** -> Maximiza todos los ítems apilables en el inventario del jugador.
- **/mute** -> Sobrescribe el comando `/mute` de TShock. Incluye subcomandos:包含子命令：
  - **add** `<nombre> <tiempo>` -> Silencia al jugador `<nombre>` por el tiempo `<tiempo>`.
  - **delete** `<nombre>` -> Quita el silencio del jugador `<nombre>`.
  - **help** -> Muestra información del comando.
- **/pvpget** -> Activa o desactiva tu estado de PvP.
- **/ruler** `[1|2]` -> Mide la distancia entre los puntos 1 y 2.
- **/sudo** `[flag] <jugador> <comando>` -> Hace que `<jugador>` ejecute `<comando>`. Bandera válida: `-force` -> Obliga al jugador a ejecutar el comando, ignorando las verificaciones de permisos. Los jugadores con el permiso `essentials.sudo.super` pueden usar `/sudo` en cualquier persona.有效标志：`-force` -> 强制执行命令，忽略 `<玩家>` 的权限限制。拥有 `essentials.sudo.super` 权限的玩家可以对任何人使用 /sudo。
- **/timecmd** `[flag] <tiempo> <comando>` -> Ejecuta `<comando>` después de `<tiempo>`. Bandera válida: `-repeat` -> Repite `<comando>` cada `<tiempo>`.有效标志：`-repeat` -> 每隔 `<时间>` 重复执行 `<命令>`。
- **/eback** `[pasos]` -> Te lleva a una ubicación previa. Si se proporciona `[pasos]`, te lleva a tu posición `[pasos]` atrás.如果提供了 `[步数]`，则尝试将您带回 `[步数]` 步之前的位置。
- **/down** `[niveles]` -> Te mueve hacia abajo en el mapa. Si se especifica `[niveles]`, intenta moverte hacia abajo `[niveles]` niveles.如果指定了 `[层数]`，则尝试向下移动 `[层数]` 次。
- **/left** `[niveles]` -> Similar a `/down [niveles]`, pero te mueve hacia la izquierda.
- **/right** `[niveles]` -> Similar a `/down [niveles]`, pero te mueve hacia la derecha.
- **/up** `[niveles]` -> Similar a `/down [niveles]`, pero te mueve hacia arriba.

## Permisos

- `essentials.find` -> Permite el uso del comando `/find`.
- `essentials.freezetime` -> Permite el uso del comando `/freezetime`.
- `essentials.home.delete` -> Permite el uso de los comandos `/delhome` y `/sethome`.
- `essentials.home.tp` -> Permite el uso del comando `/myhome`.
- `essentials.kickall` -> Permite el uso del comando `/kickall`.
- `essentials.lastcommand` -> Permite el uso del comando `/=`.
- `essentials.more` -> Permite el uso del comando `/more`.
- `essentials.mute` -> Permite el uso del comando mejorado `/mute`.
- `essentials.pvp` -> Permite el uso del comando `/pvpget`.
- `essentials.ruler` -> Permite el uso del comando `/ruler`.
- `essentials.send` -> Permite el uso del comando `/send`.
- `essentials.sudo` -> Permite el uso del comando `/sudo`.
- `essentials.sudo.force` -> Extiende las capacidades de `/sudo`.
- `essentials.sudo.super` -> Permite usar `/sudo` en cualquier persona.
- `essentials.sudo.invisible` -> Hace que los comandos ejecutados con `/sudo` sean invisibles.
- `essentials.timecmd` -> Permite el uso del comando `/timecmd`.
- `essentials.tp.back` -> Permite el uso del comando `/eback`.
- `essentials.tp.down` -> Permite el uso del comando `/down`.
- `essentials.tp.left` -> Permite el uso del comando `/left`.
- `essentials.tp.right` -> Permite el uso del comando `/right`.
- `essentials.tp.up` -> Permite el uso del comando `/up`.

## Configuración

> Ubicación del archivo de configuración: `tshock/EssentialsPlus.json`

```json5
{
  // Lista de comandos que desactivan PvP (Jugador contra Jugador) al usarlos.
  "ComandosDesactivanPvp": [
    "eback"
  ],

  // Número de ubicaciones anteriores almacenadas en el historial para funciones como retroceso.
  "HistorialDePosiciones": 10,

  // Información del servidor MySQL. Requerida si se usa MySQL como base de datos.
  "ServidorMySql": "Configurar información completa si se usa MySQL", // Dirección del host, por ejemplo, "localhost" o una dirección IP.

  // Nombre de la base de datos MySQL a utilizar.
  "NombreBaseDeDatosMySql": "", // Nombre de la base de datos, por ejemplo, "midatabase".

  // Usuario MySQL con acceso a la base de datos especificada.
  "UsuarioMySql": "", // Nombre de usuario para acceder a la base de datos, por ejemplo, "usuarioBD".

  // Contraseña para el usuario MySQL.
  "ContrasenaMySql": "" // Contraseña del usuario especificado, por ejemplo, "contraseña123".
}
```

## 更新日志

```
1.0.4
添加西班牙语，修正部分内容
1.0.3
i18n完成，且预置es-EN
1.0.2
修复数据库错误
1.0.1 
修复重启无法获取禁言的BUG, 重命名一些方法
```

## 反馈

- Github Issue -> Repositorio de TShockPlugin: https://github.com/UnrealMultiple/TShockPlugin
- Grupo de QQ de TShock: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
