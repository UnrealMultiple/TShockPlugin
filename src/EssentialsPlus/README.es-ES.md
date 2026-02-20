# EssentialsPlus

- Autores: WhiteX y otros, Average, Cjx, adaptación y modificación por 肝帝熙恩, actualización por Cai
- Fuente: [Github](https://github.com/QuiCM/EssentialsPlus)
- Essentials+ es una combinación de funcionalidades de Essentials y MoreAdminCommands mejoradas. Todos los comandos se ejecutan de manera asíncrona. No incluye comandos de señales.

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
- **/kickall** `<flag> <razón>` -> Expulsa a todos los jugadores por `<razón>`. Bandera válida: `-nosave` -> La expulsión no guarda el inventario SSC.
- **/=** -> Repite el último comando ingresado (excluyendo otros usos de /=).
- **/more** o **/stack** -> Maximiza la pila del ítem sostenido (incluyendo ítems con sufijos). Incluye subcomandos:
  - **all**  -> Maximiza todos los ítems en el inventario del jugador (solo ítems sin sufijos).
  - **fall** -> Maximiza todos los ítems en el inventario del jugador (incluyendo ítems con sufijos).
- **/mute** -> Sobrescribe el comando `/mute` de TShock. Incluye subcomandos:
  - **add** `<nombre> <tiempo>` -> Silencia al jugador `<nombre>` por el tiempo `<tiempo>`.
  - **delete** `<nombre>` -> Quita el silencio del jugador `<nombre>`.
  - **help** -> Muestra información del comando.
- **/tpallow** -> Comando mejorado `/tpallow`.
- **/pvpget** -> Activa o desactiva tu estado de PvP.
- **/ruler** `[1|2]` -> Mide la distancia entre los puntos 1 y 2.
- **/sudo** `[flag] <jugador> <comando>` -> Hace que `<jugador>` ejecute `<comando>`. Bandera válida: `-force` -> Obliga al jugador a ejecutar el comando, ignorando las verificaciones de permisos. Los jugadores con el permiso `essentials.sudo.super` pueden usar `/sudo` en cualquier persona.
- **/timecmd** `[flag] <tiempo> <comando>` -> Ejecuta `<comando>` después de `<tiempo>`. Bandera válida: `-repeat` -> Repite `<comando>` cada `<tiempo>`.
- **/eback** `[pasos]` -> Te lleva a una ubicación previa. Si se proporciona `[pasos]`, te lleva a tu posición `[pasos]` atrás.
- **/down** `[niveles]` -> Te mueve hacia abajo en el mapa. Si se especifica `[niveles]`, intenta moverte hacia abajo `[niveles]` niveles.
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
- `essentials.tpallow` -> Permite el uso del comando mejorado `/tpallow`.
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

## Soporte

- Github Issue -> Repositorio de TShockPlugin: https://github.com/UnrealMultiple/TShockPlugin
- Grupo de QQ de TShock: 816771079
- Foro de Terraria en China: trhub.cn, bbstr.net, tr.monika.love