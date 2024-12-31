# ItemDeco Plugin

- Autor: FrankV22
- Fuente: [Github](https://github.com/itsFrankV22/ItemsDeco-Plugin)
- Mostrar el nombre del ítem: Cuando un jugador cambia el ítem que sostiene, el nombre de ese ítem aparecerá como un mensaje flotante sobre su cabeza y en el chat. Esta función también muestra el daño del ítem, y se puede habilitar o deshabilitar a través del archivo de configuración.
- Personalización de colores: El color del mensaje flotante está configurado por defecto en blanco (RGB 255, 255, 255), y es personalizable.
- Compatible con [Floating-MessageChat](https://github.com/itsFrankV22/FloatingText-Chat)

## Comandos

No se requiere ningún comando adicional.

## Configuración

> Ruta del archivo de configuración: `tshock/ItemDeco/ItemDecoration.es-ES.json`
```json5
{
  "ObjetoEnChat": {
    "colorDelObjeto": {
      "R": 255,
      "G": 255,
      "B": 255
    },
    "ColorDelDaño": {
      "R": 0,
      "G": 255,
      "B": 255
    },
    "mostrarNombre": false,
    "mostrarDaño": false
  },
  "textoDeObjeto": {
    "textoDeDaño": "Damage",
    "colorPorDefecto": {
      "R": 255,
      "G": 255,
      "B": 255
    },
    "coloresDeRaridad": {
      "-1": {
        "R": 169,
        "G": 169,
        "B": 169
      },
      "0": {
        "R": 255,
        "G": 255,
        "B": 255
      },
      "1": {
        "R": 0,
        "G": 128,
        "B": 0
      },
      "2": {
        "R": 0,
        "G": 112,
        "B": 221
      },
      "3": {
        "R": 128,
        "G": 0,
        "B": 128
      },
      "4": {
        "R": 255,
        "G": 128,
        "B": 0
      },
      "5": {
        "R": 255,
        "G": 0,
        "B": 0
      },
      "6": {
        "R": 255,
        "G": 215,
        "B": 0
      },
      "7": {
        "R": 255,
        "G": 105,
        "B": 180
      },
      "8": {
        "R": 255,
        "G": 215,
        "B": 0
      },
      "9": {
        "R": 0,
        "G": 255,
        "B": 255
      },
      "10": {
        "R": 255,
        "G": 105,
        "B": 180
      },
      "11": {
        "R": 75,
        "G": 0,
        "B": 130
      }
    },
    "mostrarNombre": true,
    "mostrarDaño": true
  }
}
```

## Comentarios

- Este plugin se encuentra en el repositorio original: [Github](https://github.com/itsFrankV22/ItemsDeco-Plugin)
- Para problemas o sugerencias, por favor abre un "issue" en el repositorio compartido: https://github.com/UnrealMultiple/TShockPlugin
- Para soporte adicional, únete al grupo oficial de TShock: 816771079
- También puedes intentar en las comunidades locales: trhub.cn, bbstr.net, tr.monika.love
