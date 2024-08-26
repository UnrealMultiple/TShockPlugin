# Economics.projectile custom barrage

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Shao Si Ming
- Source: None
- Custom barrage makes your weapon more cool

> [! Note]
> Need to install front plug -ins: Economicsapi, Economics.rpg (this warehouse)

## Update log

```
无
```

## instruction

none

## Configuration
> Configuration file location: TSHOCK/ECONOMICS/Projectile.json
```json
{
   "弹幕触发": {
     "274": {
       "使用物品时触发": true, //注意下面的布尔值，严格分类不要弄混了
       "来自于召唤物": false,
       "本身就是召唤物": false,
       "召唤物攻击间隔": 15,
       "弹幕数据": [
        {
           "弹幕ID": 132,
           "弹幕击退动态跟随": false,
           "弹幕伤害动态跟随": false,
           "弹幕伤害": 80.0,
           "弹幕击退": 10.0,
           "弹幕射速": 15.0,
           "限制等级": []
        }
      ],
       "备注": "" 
    }
  },
   "物品触发 ": {
     "1327": {
       "弹幕数据": [
        {
           "弹幕ID": 132,
           "弹幕击退动态跟随": false,
           "弹幕伤害动态跟随": false,
           "弹幕伤害": 80.0,
           "弹幕击退": 10.0,
           "弹幕射速": 15.0,
           "限制等级": []
        }
      ],
       "物品使用弹药": false,
       "备注": "" 
    }
  }
}
```

## feedback

- Co -maintained plug -in library: https://github.com/Controllerdestiny/tshockplugin
- The official group of Trhub.cn or Tshock in the domestic community