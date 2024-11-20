# ItemDeco Plugin

- Autor: FrankV22
- Fuente: [Github](https://github.com/itsFrankV22/ItemsDeco-Plugin)
- Mostrar el nombre del ítem: Cuando un jugador cambia el ítem que sostiene, el nombre de ese ítem aparecerá como un mensaje flotante sobre su cabeza y en el chat. Esta función también muestra el daño del ítem, y se puede habilitar o deshabilitar a través del archivo de configuración.
- Personalización de colores: El color del mensaje flotante está configurado por defecto en blanco (RGB 255, 255, 255), y es personalizable.
- Compatible con [Floating-MessageChat](https://github.com/itsFrankV22/FloatingText-Chat)

## Registro de cambios

Ninguno por ahora.

## Comandos

No se requiere ningún comando adicional.

## Configuración

> Ruta del archivo de configuración: `tshock/ItemDeco/ItemChatConfig.json`
```json
{
  "CONFIGURATION": {    
    "ShowItem": true, // Si se muestra el nombre del ítem
    "ShowDamage": true, // Si se muestra el daño del ítem
    "ItemColor": {  // Color del nombre del ítem
      "R": 255,
      "G": 255,
      "B": 255
    },
    "DamageColor": {  // Color del daño
      "R": 0,
      "G": 255,
      "B": 255
    }
  }
}
```

> Ruta del archivo de configuración: `tshock/ItemDeco/ItemTextConfig.json`
```json
{
  "ShowName": true, // Si se muestra el nombre del ítem
  "ShowDamage": true, // Si se muestra el daño del ítem
  "DamageText": "Daño", // Texto que precede al valor de daño
  "DefaultColor": { // Color predeterminado
    "r": 255, // Componente rojo
    "g": 255, // Componente verde
    "b": 255  // Componente azul
  },
  "RarityColors": { // Colores para la rareza
    "-1": { // Gris (169, 169, 169)
      "r": 169,
      "g": 169,
      "b": 169
    },
    "0": { // Blanco (255, 255, 255)
      "r": 255,
      "g": 255,
      "b": 255
    },
    "1": { // Verde (0, 128, 0)
      "r": 0,
      "g": 128,
      "b": 0
    },
    "2": { // Azul (0, 112, 221)
      "r": 0,
      "g": 112,
      "b": 221
    },
    "3": { // Púrpura (128, 0, 128)
      "r": 128,
      "g": 0,
      "b": 128
    },
    "4": { // Naranja (255, 128, 0)
      "r": 255,
      "g": 128,
      "b": 0
    },
    "5": { // Rojo (255, 0, 0)
      "r": 255,
      "g": 0,
      "b": 0
    },
    "6": { // Dorado (255, 215, 0)
      "r": 255,
      "g": 215,
      "b": 0
    },
    "7": { // Rosa (255, 105, 180)
      "r": 255,
      "g": 105,
      "b": 180
    },
    "8": { // Dorado (255, 215, 0)
      "r": 255,
      "g": 215,
      "b": 0
    },
    "9": { // Cian (0, 255, 255)
      "r": 0,
      "g": 255,
      "b": 255
    },
    "10": { // Rosa (255, 105, 180)
      "r": 255,
      "g": 105,
      "b": 180
    },
    "11": { // Púrpura oscuro (75, 0, 130)
      "r": 75,
      "g": 0,
      "b": 130
    }
  }
}
```

## Comentarios

- Este plugin se encuentra en el repositorio original: [Github](https://github.com/itsFrankV22/ItemsDeco-Plugin)
- Para problemas o sugerencias, por favor abre un "issue" en el repositorio compartido: https://github.com/UnrealMultiple/TShockPlugin
- Para soporte adicional, únete al grupo oficial de TShock: 816771079
- También puedes intentar en las comunidades locales: trhub.cn, bbstr.net, tr.monika.love
