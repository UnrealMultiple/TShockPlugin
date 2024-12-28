<div align="center">
  
[![TShockPlugin](https://socialify.git.ci/UnrealMultiple/TShockPlugin/image?description=1&descriptionEditable=A%20TShock%20Chinese%20Plugin%20Collection%20Repository&forks=1&issues=1&language=1&logo=https%3A%2F%2Fgithub.com%2FUnrealMultiple%2FTShockPlugin%2Fblob%2Fmaster%2Ficon.png%3Fraw%3Dtrue&name=1&pattern=Circuit%20Board&pulls=1&stargazers=1&theme=Auto)](https://github.com/UnrealMultiple/TShockPlugin)  
[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/UnrealMultiple/TShockPlugin/.github%2Fworkflows%2Fbuild.yml)](https://github.com/UnrealMultiple/TShockPlugin/actions)
[![GitHub contributors](https://img.shields.io/github/contributors/UnrealMultiple/TShockPlugin?style=flat)](https://github.com/UnrealMultiple/TShockPlugin/graphs/contributors)
[![NET6](https://img.shields.io/badge/Core-%20.NET_6-blue)](https://dotnet.microsoft.com/zh-cn/)
[![QQ](https://img.shields.io/badge/QQ-EB1923?logo=tencent-qq&logoColor=white)](https://qm.qq.com/cgi-bin/qm/qr?k=54tOesIU5g13yVBNFIuMBQ6AzjgE6f0m&jump_from=webapi&authKey=6jzafzJEqQGzq7b2mAHBw+Ws5uOdl83iIu7CvFmrfm/Xxbo2kNHKSNXJvDGYxhSW)
[![TShock](https://img.shields.io/badge/TShock5.2.0-2B579A.svg?&logo=TShock&logoColor=white)](https://github.com/Pryaxis/TShock)

[简体中文](README.md) | [English](README.en-US.md) | **&gt; Spanish/Español &lt;**

</div>

## Introduccion
- Este es un repositorio dedicado a recopilar e integrar complementos "TShock".
- Algunos de los complementos de la biblioteca se recopilan de Internet y se descompilan.
- Debido a la naturaleza especial del proyecto, puede causar infracción. Si hay alguna infracción, por favor contáctenos para resolverla.
- Continuaremos recopilando complementos de "TShock" de alta calidad, actualizándolos de manera oportuna y estando al día con la última versión de "TShock".
- Si desea unirse a nosotros, siga las "Notas del desarrollador" y envíe una "Solicitud de extracción" a este repositorio.


## Notas de usuario

- Tenga en cuenta que algunos complementos pueden requerir dependencias; consulte la lista a continuación para instalar las dependencias.
- Cada complemento tiene una nota de uso; haga clic en el hipervínculo en la lista a continuación para ver las instrucciones específicas.
- Se dice que a las personas a las que les gusta destacar repositorios, sus complementos no son fáciles de generar errores.

## Descargas

- Github Release: [Plugins.zip](https://github.com/UnrealMultiple/TShockPlugin/releases/download/V1.0.0.0/Plugins.zip)
- Gitee Release: [Plugins.zip](https://gitee.com/kksjsj/TShockPlugin/releases/download/V1.0.0.0/Plugins.zip)

#### AutoPluginManager
    /apm l Lista todos los plugins
    /apm i <plugin name> Una vez - Instalar Plugin
    /apm u [plugin name] Ver si el plugin tiene actualizacion

## Notas de Desarrollador

> Estandarización de codigo

- Está prohibido el uso de variables chinas.
- No utilice funciones peligrosas.
- Evite el uso de subprocesos múltiples siempre que sea posible.
- No dejar puertas traseras en los complementos.
- Incluya un archivo README.md con cada proyecto de complemento.

## Comentarios

> Cualquier comentario, sugerencia o mejora en esta biblioteca de códigos se considerará contribución pública y podrá incluirse en esta biblioteca de códigos a menos que se indique explícitamente lo contrario.

- Si hay un error, proporcione la información relevante del sistema, la versión de TShock y el proceso de reproducción del error en la página "problema" de GitHub.

### Plugins Recolectados

> Haga clic en los hipervínculos para ver la descripción detallada del complemento

> [!NOTE]
> Es posible que la documentación del complemento en inglés no se actualice tan rápidamente como la documentación del complemento en chino.
> Intente consultar la documentación china siempre que sea posible.

<Details>
<Summary>Listado de Plugins</Summary>

| Nombre del plugin | Disponible en Español | Descripcion del Plugin  | Dependencias |
| :-: | :-: | :-: | :-: |
| [AdditionalPylons](./src/AdditionalPylons/README.md) | No | Colocar más pilones | [LazyAPI](./src/LazyAPI/README.md) |
| [AnnouncementBoxPlus](./src/AnnouncementBoxPlus/README.md) | No | Mejora la funcionalidad de la caja de anuncios | [LazyAPI](./src/LazyAPI/README.md) |
| [AutoAirItem](./src/AutoAirItem/README.md) | No | Botes de basura automáticos | [LazyAPI](./src/LazyAPI/README.md) |
| [AutoBroadcast](./src/AutoBroadcast/README.md) | No | Transmisión automática | [LazyAPI](./src/LazyAPI/README.md) |
| [AutoClear](./src/AutoClear/README.md) | No | Limpieza automática inteligente | [LazyAPI](./src/LazyAPI/README.md) |
| [AutoFish](./src/AutoFish/README.md) | No | Pesca automática | [LazyAPI](./src/LazyAPI/README.md) |
| [AutoPluginManager](./src/AutoPluginManager/README.es-ES.md) | Si | Actualice los complementos automáticamente con una sola tecla |  |
| [AutoReset](./src/AutoReset/README.md) | No | Reinicio completamente automático | [LazyAPI](./src/LazyAPI/README.md) |
| [AutoStoreItems](./src/AutoStoreItems/README.md) | No | Almacenamiento automático | [LazyAPI](./src/LazyAPI/README.md) |
| [AutoTeam](./src/AutoTeam/README.md) | No | Formación automática de equipos | [LazyAPI](./src/LazyAPI/README.md) |
| [Back](./src/Back/README.md) | No | Regresar al punto de muerte | [LazyAPI](./src/LazyAPI/README.md) |
| [BagPing](./src/BagPing/README.md) | No | Marcar las bolsas de tesoro en el mapa |  |
| [BanNpc](./src/BanNpc/README.md) | No | Previene la generación de monstruos | [LazyAPI](./src/LazyAPI/README.md) |
| [BedSet](./src/BedSet/README.md) | No | Establecer y registrar puntos de resurrección | [LazyAPI](./src/LazyAPI/README.md) |
| [BetterWhitelist](./src/BetterWhitelist/README.md) | No | Plugin de lista blanca | [LazyAPI](./src/LazyAPI/README.md) |
| [BridgeBuilder](./src/BridgeBuilder/README.md) | No | Construcción rápida de puentes | [LazyAPI](./src/LazyAPI/README.md) |
| [BuildMaster](./src/BuildMaster/README.md) | No | Modo Maestro Constructor para el Mini Juego Red Bean | [MiniGamesAPI](./src/MiniGamesAPI/README.md) |
| [CaiBot](./src/CaiBot/README.md) | No | Plugin adaptador CaiBot (Only support QQ) |  |
| [CaiCustomEmojiCommand](./src/CaiCustomEmojiCommand/README.md) | No | Comando de emoji personalizado | [LazyAPI](./src/LazyAPI/README.md) |
| [CaiLib](./src/CaiLib/README.md) | No | Biblioteca de precarga de Cai | [SixLabors.ImageSharp]() |
| [CaiPacketDebug](./src/CaiPacketDebug/README.md) | No | Herramienta de depuración de paquetes Cai | [LazyAPI](./src/LazyAPI/README.md) [TrProtocol]() |
| [CaiRewardChest](./src/CaiRewardChest/README.md) | No | Convierte cofres generados naturalmente en cofres de recompensa que todos pueden reclamar una vez | [linq2db]() [LazyAPI](./src/LazyAPI/README.md) |
| [CGive](./src/CGive/README.md) | No | Comandos fuera de línea |  |
| [Challenger](./src/Challenger/README.md) | Si | Modo Challenger |  |
| [Chameleon](./src/Chameleon/README.md) | No | Inicia sesión antes de entrar al servidor | [LazyAPI](./src/LazyAPI/README.md) |
| [ChattyBridge](./src/ChattyBridge/README.md) | No | Usado para el chat entre servidores | [LazyAPI](./src/LazyAPI/README.md) |
| [ChestRestore](./src/ChestRestore/README.md) | No | Objetos infinitos en servidores de recursos |  |
| [Chireiden.TShock.Omni](https://github.com/sgkoishi/yaaiomni/blob/master/README.md) | No | Otro plugin misceláneo para TShock - la parte central |  |
| [Chireiden.TShock.Omni.Misc](https://github.com/sgkoishi/yaaiomni/blob/master/README.md) | No | Otro plugin misceláneo para TShock - la parte miscelánea | [Chireiden.TShock.Omni](https://github.com/sgkoishi/yaaiomni/blob/master/README.md) |
| [CNPCShop](./src/CNPCShop/README.md) | No | Tienda personalizada de NPC |  |
| [ConsoleSql](./src/ConsoleSql/README.md) | No | Ejecutar sentencias SQL en la consola |  |
| [ConvertWorld](./src/ConvertWorld/README.md) | No | Convertir objetos del mundo al derrotar monstruos |  |
| [CreateSpawn](./src/CreateSpawn/README.md) | No | Generación de puntos de aparición | [LazyAPI](./src/LazyAPI/README.md) |
| [CriticalHit](./src/CriticalHit/README.md) | No | Indicación de golpe crítico |  |
| [Crossplay](https://github.com/UnrealMultiple/Crossplay/blob/main/README.md) | No | Permite el juego multiplataforma |  |
| [CustomMonster](./src/CustomMonster/README.md) | No | Personalizar, modificar y generar monstruos y jefes  |  |
| [DamageRuleLoot](./src/DamageRuleLoot/README.md) | No | Determinar la bolsa de tesoro caída basada en la relación de daño y transferir el cálculo de daño |  |
| [DamageStatistic](./src/DamageStatistic/README.md) | No | Mostrar el daño causado por cada jugador después de cada pelea de jefe |  |
| [DataSync](./src/DataSync/README.md) | No | Sincronización de progreso |  |
| [DeathDrop](./src/DeathDrop/README.md) | No | Botín aleatorio y personalizado al morir un monstruo |  |
| [DisableMonsLoot](./src/DisableMonsLoot/README.md) | No | Prohibir el botín de monstruos |  |
| [DonotFuck](./src/DonotFuck/README.md) | No | Prevenir groserías | [LazyAPI](./src/LazyAPI/README.md) |
| [DTEntryBlock](./src/DTEntryBlock/README.md) | No | Prevenir la entrada a mazmorras o templos |  |
| [DumpTerrariaID](./src/DumpTerrariaID/README.md) | No | Volcar las ID de Terraria |  |
| [DwTP](./src/DwTP/README.md) | No | Teletransportación por posicionamiento |  |
| [Economics.Deal](./src/Economics.Deal/README.md) | No | Plugin de comercio | [EconomicsAPI](./src/EconomicsAPI/README.md) |
| [Economics.NPC](./src/Economics.NPC/README.md) | No | Recompensas personalizadas de monstruos | [EconomicsAPI](./src/EconomicsAPI/README.md) |
| [Economics.Projectile](./src/Economics.Projectile/README.md) | No | Proyectiles personalizados | [EconomicsAPI](./src/EconomicsAPI/README.md) [Economics.RPG](./src/Economics.RPG/README.md) |
| [Economics.Regain](./src/Economics.Regain/README.md) | No | Reciclaje de objetos | [EconomicsAPI](./src/EconomicsAPI/README.md) |
| [Economics.RPG](./src/Economics.RPG/README.md) | No | Plugin RPG | [EconomicsAPI](./src/EconomicsAPI/README.md) |
| [Economics.Shop](./src/Economics.Shop/README.md) | No | Plugin de tienda | [EconomicsAPI](./src/EconomicsAPI/README.md) [Economics.RPG](./src/Economics.RPG/README.md) |
| [Economics.Skill](./src/Economics.Skill/README.md) | No | Plugin de habilidades | [EconomicsAPI](./src/EconomicsAPI/README.md) [Jint]() [Economics.RPG](./src/Economics.RPG/README.md) |
| [Economics.Task](./src/Economics.Task/README.md) | No | Plugin de tareas | [EconomicsAPI](./src/EconomicsAPI/README.md) [Economics.RPG](./src/Economics.RPG/README.md) |
| [Economics.WeaponPlus](./src/Economics.WeaponPlus/README.md) | No | Mejora de armas | [EconomicsAPI](./src/EconomicsAPI/README.md) |
| [EconomicsAPI](./src/EconomicsAPI/README.md) | No | Plugin económico |  |
| [EndureBoost](./src/EndureBoost/README.md) | No | Otorga un buff específico cuando el jugador tiene una cantidad determinada de objetos |  |
| [EssentialsPlus](./src/EssentialsPlus/README.es-ES.md) | Si | Comandos de gestión adicionales |  |
| [Ezperm](./src/Ezperm/README.md) | No | Cambio por lotes de permisos |  |
| [FishShop](https://github.com/UnrealMultiple/TShockFishShop/blob/master/README.md) | No | Tienda de peces |  |
| [GenerateMap](./src/GenerateMap/README.md) | No | Generar imágenes de mapas | [CaiLib](./src/CaiLib/README.md) |
| [GolfRewards](./src/GolfRewards/README.md) | No | Recompensas de golf |  |
| [GoodNight](./src/GoodNight/README.md) | No | Toque de queda |  |
| [HardPlayerDrop](./src/HardPlayerDrop/README.md) | No | Los jugadores en modo Hardcore sueltan corazones de vida al morir |  |
| [HelpPlus](./src/HelpPlus/README.md) | No | Corrige y mejora el comando de ayuda |  |
| [History](./src/History/README.md) | No | Registra un historial en formato de tabla |  |
| [HouseRegion](./src/HouseRegion/README.md) | No | Plugin de reclamación de tierras |  |
| [Invincibility](./src/Invincibility/README.md) | No | Invencibilidad limitada en el tiempo |  |
| [ItemBox](./src/ItemBox/README.md) | No | Inventario fuera de línea |  |
| [ItemDecoration](./src/ItemDecoration/README.es-ES.md) | No | Muestra mensajes flotantes para los ítems en las manos | [LazyAPI](./src/LazyAPI/README.md) |
| [ItemPreserver](./src/ItemPreserver/README.md) | No | Conserva ítems específicos de la consumición |  |
| [JourneyUnlock](./src/JourneyUnlock/README.md) | No | Desbloquea ítems del modo Journey |  |
| [Lagrange.XocMat.Adapter](./src/Lagrange.XocMat.Adapter/README.md) | No | Plugin adaptador para el bot Lagrange.XocMat | [SixLabors.ImageSharp]() |
| [LazyAPI](./src/LazyAPI/README.md) | No | Biblioteca base para plugins | [linq2db]() |
| [LifemaxExtra](./src/LifemaxExtra/README.md) | No | Comer más frutas/cristales de vida | [LazyAPI](./src/LazyAPI/README.md) |
| [ListPlugins](./src/ListPlugins/README.md) | No | Lista los plugins instalados |  |
| [MapTp](./src/MapTp/README.md) | No | Teletransportarse con doble clic en el mapa |  |
| [MiniGamesAPI](./src/MiniGamesAPI/README.md) | No | API para el mini-juego de pasta de frijol |  |
| [ModifyWeapons](./src/ModifyWeapons/README.md) | No | Deje que los jugadores realicen dos Sprint | [LazyAPI](./src/LazyAPI/README.md) |
| [MonsterRegen](./src/MonsterRegen/README.md) | No | Regeneración de progreso de monstruos |  |
| [MusicPlayer](./src/MusicPlayer/README.md) | No | Reproductor de música simple |  |
| [Noagent](./src/Noagent/README.md) | No | Prohíbe que las IPs de proxy ingresen al servidor |  |
| [NormalDropsBags](./src/NormalDropsBags/README.md) | No | Suelta bolsas de tesoros en dificultad normal |  |
| [OnlineGiftPackage](./src/OnlineGiftPackage/README.md) | No | Paquete de regalos en línea |  |
| [PacketsStop](./src/PacketsStop/README.md) | No | Interceptación de paquetes |  |
| [PermaBuff](./src/PermaBuff/README.md) | No | Buff permanente |  |
| [PerPlayerLoot](./src/PerPlayerLoot/README.md) | No | Cofre separado para el botín del jugador |  |
| [PersonalPermission](./src/PersonalPermission/README.md) | No | Establece permisos individualmente para los jugadores |  |
| [Platform](./src/Platform/README.md) | No | Determina el dispositivo del jugador |  |
| [PlayerManager](https://github.com/UnrealMultiple/TShockPlayerManager/blob/master/README.md) | No | Administrador de jugadores de Hufang |  |
| [PlayerSpeed](./src/PlayerSpeed/README.md) | No | Interceptación de paquetes | [LazyAPI](./src/LazyAPI/README.md) |
| [ProgressBag](./src/ProgressBag/README.md) | No | Paquete de progreso |  |
| [ProgressControls](./src/ProgressControls/README.md) | No | Planificador (Automatiza el control del servidor) |  |
| [ProgressRestrict](./src/ProgressRestrict/README.md) | No | Detección de super progreso | [DataSync](./src/DataSync/README.md) |
| [ProxyProtocolSocket](./src/ProxyProtocolSocket/README.md) | No | Acepta conexiones de protocolo proxy |  |
| [PvPer](./src/PvPer/README.md) | No | Sistema de duelos |  |
| [RainbowChat](./src/RainbowChat/README.md) | No | Colores aleatorios en el chat |  |
| [RandomBroadcast](./src/RandomBroadcast/README.md) | No | Transmisión aleatoria |  |
| [RandRespawn](./src/RandRespawn/README.md) | No | Punto de aparición aleatorio |  |
| [RealTime](./src/RealTime/README.md) | No | Sincroniza la hora del servidor con la hora real |  |
| [RebirthCoin](./src/RebirthCoin/README.md) | No | Consume ítems designados para revivir al jugador |  |
| [RecipesBrowser](./src/RecipesBrowser/README.md) | No | Mesa de trabajo |  |
| [ReFishTask](./src/ReFishTask/README.md) | No | Refresca automáticamente las tareas del pescador |  |
| [RegionView](./src/RegionView/README.md) | No | Muestra los límites de las áreas |  |
| [Respawn](./src/Respawn/README.md) | No | Reaparece en el lugar de la muerte |  |
| [RestInventory](./src/RestInventory/README.md) | No | Proporciona una interfaz de consulta REST para la mochila |  |
| [RolesModifying](./src/RolesModifying/README.md) | No | Modificar mochila del jugador |  |
| [Sandstorm](./src/Sandstorm/README.md) | No | Alterna la tormenta de arena |  |
| [ServerTools](./src/ServerTools/README.md) | No | Herramientas de administración del servidor | [LazyAPI](./src/LazyAPI/README.md) [linq2db]() |
| [SessionSentinel](./src/SessionSentinel/README.md) | No | Maneja jugadores que no envían paquetes de datos por mucho tiempo |  |
| [ShortCommand](./src/ShortCommand/README.md) | No | Comando corto |  |
| [ShowArmors](./src/ShowArmors/README.md) | No | Muestra la barra de equipo |  |
| [SignInSign](./src/SignInSign/README.md) | No | Plugin de inicio de sesión con cartel |  |
| [SimultaneousUseFix](./src/SimultaneousUseFix/README.md) | No | Resuelve problemas como el martillo doble atascado y la metralleta de estrellas | [Chireiden.TShock.Omni](https://github.com/sgkoishi/yaaiomni/blob/master/README.md) |
| [SmartRegions](./src/SmartRegions/README.md) | No | Regiones inteligentes |  |
| [SpawnInfra](./src/SpawnInfra/README.md) | No | Genera infraestructura básica |  |
| [SpclPerm](./src/SpclPerm/README.md) | No | Privilegios del propietario del servidor |  |
| [StatusTextManager](./src/StatusTextManager/README.md) | No | Plugin para gestionar el texto de estado en PC |  |
| [SurfaceBlock](./src/SurfaceBlock/README.md) | No | Prohibir proyectiles en la superficie  | [LazyAPI](./src/LazyAPI/README.md) |
| [SwitchCommands](./src/SwitchCommands/README.md) | No | Ejecuta comandos en regiones |  |
| [TeleportRequest](./src/TeleportRequest/README.md) | No | Solicitud de teletransporte |  |
| [TimeRate](./src/TimeRate/README.md) | No | Modifica la aceleración del tiempo usando comandos, y soporta el sueño de los jugadores para activar eventos |  |
| [TimerKeeper](./src/TimerKeeper/README.md) | No | Guarda el estado del temporizador |  |
| [TownNPCHomes](./src/TownNPCHomes/README.md) | No | Casa rápida de NPC |  |
| [TShockConfigMultiLang](./src/TShockConfigMultiLang/README.md) | No | Localización del idioma de configuración de TShock | [LazyAPI](./src/LazyAPI/README.md) |
| [UnseenInventory](./src/UnseenInventory/README.md) | No | Permite que el servidor genere ítems "inobtenibles" |  |
| [VeinMiner](./src/VeinMiner/README.md) | No | Minado en cadena |  |
| [VotePlus](./src/VotePlus/README.md) | No | Votación multifuncional |  |
| [WeaponPlus](./src/WeaponPlusCostCoin/README.md) | No | Versión de monedas para mejorar armas |  |
| [WikiLangPackLoader](./src/WikiLangPackLoader/README.md) | No | 为服务器加载 Wiki 语言包 |  |
| [WorldModify](https://github.com/UnrealMultiple/TShockWorldModify/blob/master/README.md) | No | Editor del mundo, permite modificar la mayoría de los parámetros del mundo |  |
| [ZHIPlayerManager](./src/ZHIPlayerManager/README.md) | No | Plugin de gestión de jugadores de Zhi |  |

</Details>

## Links

- [TShock Plugin Development Documentation](https://github.com/ACaiCat/TShockPluginDocument)
- [Tshock Comprehensive Navigation](https://github.com/UnrealMultiple/Tshock-nav)
