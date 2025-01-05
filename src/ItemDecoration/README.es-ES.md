# ItemDeco 手持物品显示

- Autor: FrankV22
- Fuente: [Github](https://github.com/itsFrankV22/ItemsDeco-Plugin)
- 显示物品名称：当玩家切换持有的物品时，该物品的名称会以浮动消息的形式出现在玩家头顶，并在聊天中显示。此功能还支持显示伤害值，并可通过配置文件启用或禁用。
- Personalización de colores: El color del mensaje flotante está configurado por defecto en blanco (RGB 255, 255, 255), y es personalizable.
- Compatible con [Floating-MessageChat](https://github.com/itsFrankV22/FloatingText-Chat)

## Comandos

```
暂无
```

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

## 更新日志

```
v1.0.0.1
完成西班牙语的i18n config配置，修复聊天不显示名字的问题
V1.0.0.0
重构代码
```

## 反馈

- Este plugin se encuentra en el repositorio original: [Github](https://github.com/itsFrankV22/ItemsDeco-Plugin)
- Para problemas o sugerencias, por favor abre un "issue" en el repositorio compartido: https://github.com/UnrealMultiple/TShockPlugin
- Para soporte adicional, únete al grupo oficial de TShock: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
